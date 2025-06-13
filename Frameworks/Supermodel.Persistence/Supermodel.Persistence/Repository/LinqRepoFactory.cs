using Supermodel.Persistence.Entities;

namespace Supermodel.Persistence.Repository;

public static class LinqRepoFactory
{
    public static ILinqDataRepo<TEntity> Create<TEntity>() where TEntity : class, IEntity, new()
    {
        return (ILinqDataRepo<TEntity>)RepoFactory.Create<TEntity>();
    }
}