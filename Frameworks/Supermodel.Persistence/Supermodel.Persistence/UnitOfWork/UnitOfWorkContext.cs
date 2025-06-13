using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.LogicalContext;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;

namespace Supermodel.Persistence.UnitOfWork;

public class UnitOfWorkContext
{
    #region Constructors
    protected UnitOfWorkContext(){ }
    #endregion
        
    #region Methods and Properties
    public static TEntity CloneDetached<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
    {
        return UnitOfWorkContextCore.CurrentDataContext.CloneDetached(entity);
    }
    public static Task SeedDataAsync()
    {
        return UnitOfWorkContextCore.CurrentDataContext.SeedDataAsync();
    }
    public static Task<int> SaveChangesAsync()
    {
        return UnitOfWorkContextCore.CurrentDataContext.SaveChangesAsync();
    }
    public static Task<int> FinalSaveChangesAsync()
    {
        return UnitOfWorkContextCore.CurrentDataContext.FinalSaveChangesAsync();
    }
    public static bool CommitOnDispose
    {
        get => UnitOfWorkContextCore.CurrentDataContext.CommitOnDispose;
        set => UnitOfWorkContextCore.CurrentDataContext.CommitOnDispose = value;
    }
    #endregion

    #region Properties
    public static ConcurrentDictionary<string, object?> CustomValues => UnitOfWorkContextCore.CurrentDataContext.CustomValues;
    #endregion
}
    
public static class UnitOfWorkContext<TDataContext> where TDataContext : class, IDataContext, new()
{
    #region Methods and Properties
    public static TDataContext PopDataContext()
    {
        return (TDataContext)UnitOfWorkContextCore.PopDataContext();
    }
    public static void PushDataContext(TDataContext context)
    {
        UnitOfWorkContextCore.PushDataContext(context);
    }
    public static int StackCount => UnitOfWorkContextCore.StackCount;

    public static TDataContext CurrentDataContext => (TDataContext)UnitOfWorkContextCore.CurrentDataContext;

    public static bool HasDataContext()
    {
        return StackCount > 0;
    }
    #endregion
}

public static class UnitOfWorkContextCore
{
    #region Methods and Properties
    public static IDataContext PopDataContext()
    {
        try
        {
            _contextStackImmutable = _contextStackImmutable.Pop(out var context);
            return context;

        }
        catch (InvalidOperationException)
        {
            throw new InvalidOperationException("Stack is empty");
        }
    }
    public static void PushDataContext(IDataContext context)
    {
        _contextStackImmutable = _contextStackImmutable.Push(context);
    }
    public static int StackCount => _contextStackImmutable.Count();
    public static IDataContext CurrentDataContext
    {
        get
        {
            try
            {
                return _contextStackImmutable.Peek();
            }
            catch (InvalidOperationException)
            {
                throw new SupermodelException("Current UnitOfWork does not exist. All database access operations must be wrapped in 'using(new UnitOfWork())'");
            }
        }
    }
    #endregion

    #region Private variables
    // ReSharper disable once InconsistentNaming
    private static ImmutableStack<IDataContext> _contextStackImmutable
    {
        get
        {
            var contextStack = DataCallContext.LogicalGetData(SupermodelDataContextStack) as ImmutableStack<IDataContext>;
            return contextStack ?? ImmutableStack.Create<IDataContext>();
        }
        set => DataCallContext.LogicalSetData(SupermodelDataContextStack, value);
    }
    public const string SupermodelDataContextStack = "SupermodelDataContextStack";
    #endregion
}