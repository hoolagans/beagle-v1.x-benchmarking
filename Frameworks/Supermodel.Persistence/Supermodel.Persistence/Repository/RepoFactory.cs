using System;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Persistence.Repository;

public static class RepoFactory
{
    public static IDataRepo<TEntity> Create<TEntity>() where TEntity : class, IEntity, new()
    {
        return UnitOfWorkContextCore.CurrentDataContext.CreateRepo<TEntity>();
    }
    public static IDataRepo CreateForRuntimeType(Type modelType)
    {
        return UnitOfWorkContextCore.CurrentDataContext.CreateRepoForRuntimeType(modelType);
    }
}