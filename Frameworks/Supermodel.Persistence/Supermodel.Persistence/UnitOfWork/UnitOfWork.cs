using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.DataContext;

namespace Supermodel.Persistence.UnitOfWork;

public class UnitOfWork<TDataContext> : IAsyncDisposable where TDataContext : class, IDataContext, new()
{
    #region Constructors
    public UnitOfWork(ReadOnly readOnly = ReadOnly.No)
    {
        Context = new TDataContext();
        if (readOnly == ReadOnly.Yes) Context.MakeReadOnly();
        UnitOfWorkContext<TDataContext>.PushDataContext(Context);
    }
    #endregion

    #region IAsyncDisposable implementation
    public async ValueTask DisposeAsync()
    {
        var context = UnitOfWorkContext<TDataContext>.PopDataContext();
        if (context != Context) throw new SupermodelException("UnitOfWork: POP on Dispose popped mismatched DB Context.");
        await context.DisposeAsync();
    }
    #endregion

    #region Properties
    public TDataContext Context { get; }
    #endregion
}