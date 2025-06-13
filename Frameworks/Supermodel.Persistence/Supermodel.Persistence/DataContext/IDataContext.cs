using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;

namespace Supermodel.Persistence.DataContext;

public interface IDataContext : IAsyncDisposable
{
    #region Methods
    Task SeedDataAsync();
        
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
    Task<int> FinalSaveChangesAsync(CancellationToken cancellationToken = new());

    IQueryable<TEntity> Items<TEntity>() where TEntity : class, IEntity, new();
        
    IDataRepo<TEntity> CreateRepo<TEntity>() where TEntity : class, IEntity, new();
    IDataRepo CreateRepoForRuntimeType(Type modelType);

    IDataContextTransaction BeginTransaction();
    #endregion

    #region Properties
    bool CommitOnDispose { get; set; }
        
    bool IsReadOnly { get; }
    void MakeReadOnly();
        
    bool IsCompletedAndFinalized { get; }
    void MakeCompletedAndFinalized();
    ConcurrentDictionary<string, object?> CustomValues { get; }

    TEntity CloneDetached<TEntity>(TEntity entity) where TEntity : class, IEntity, new();
    #endregion
}