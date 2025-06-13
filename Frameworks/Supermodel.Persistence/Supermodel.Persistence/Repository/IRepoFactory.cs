using Supermodel.Persistence.Entities;

namespace Supermodel.Persistence.Repository;

public interface IRepoFactory
{
    IDataRepo<TEntity>? CreateRepo<TEntity>() where TEntity : class, IEntity, new();
}