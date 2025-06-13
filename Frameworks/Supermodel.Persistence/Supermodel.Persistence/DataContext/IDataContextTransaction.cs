using System;

namespace Supermodel.Persistence.DataContext;

//We intentionally do not implement IAsyncDisposable here. Dispose is never async because we commit in a separate method
public interface IDataContextTransaction : IDisposable 
{
    Guid TransactionGuid { get; }
    void Commit();
    void Rollback();
}