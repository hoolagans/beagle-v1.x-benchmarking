using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.DataContext;

namespace Supermodel.Persistence.UnitOfWork;

public class UnitOfWorkIfNoAmbientContext<TDataContext> : IAsyncDisposable/*, IDisposable*/ where TDataContext : class, IDataContext, new()
{
    #region Constructors
    public UnitOfWorkIfNoAmbientContext(MustBeWritable mustBeWritable)
    {
        bool? createNewContext;
        var mustBeWritableBool = (mustBeWritable == MustBeWritable.Yes);

        if (UnitOfWorkContextCore.StackCount > 0 && UnitOfWorkContextCore.CurrentDataContext.GetType() == typeof(TDataContext))
        {
            var ambientContextReadOnlyBool = UnitOfWorkContext<TDataContext>.CurrentDataContext.IsReadOnly;
            if (mustBeWritableBool && ambientContextReadOnlyBool) createNewContext = true;
            else createNewContext = false;
        }
        else
        {
            createNewContext = true;
        }

        if ((bool)createNewContext)
        {
            DataContext = new TDataContext();
            if (!mustBeWritableBool) DataContext.MakeReadOnly();
            UnitOfWorkContext<TDataContext>.PushDataContext(DataContext);
        }
        else
        {
            DataContext = null;
        }
    }
    #endregion

    #region IAsyncDisposable and IDisposable implementation
    public async ValueTask DisposeAsync()
    {
        if (DataContext == null) return;

        await DataContext.DisposeAsync();

        var dbContext = UnitOfWorkContext<TDataContext>.PopDataContext();
        // ReSharper disable PossibleUnintendedReferenceComparison
        if (dbContext != DataContext) throw new SupermodelException("UnitOfWork: POP on Dispose popped mismatched DataContext.");
        // ReSharper restore PossibleUnintendedReferenceComparison
    }
    #endregion

    #region Properties
    public TDataContext? DataContext { get; }
    #endregion
}