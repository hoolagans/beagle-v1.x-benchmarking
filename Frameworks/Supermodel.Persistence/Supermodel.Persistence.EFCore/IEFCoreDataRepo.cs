using Microsoft.EntityFrameworkCore;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;

namespace Supermodel.Persistence.EFCore;

public interface IEFCoreDataRepo<TEntity> : ILinqDataRepo<TEntity> where TEntity : class, IEntity, new()
{
    DbSet<TEntity> DbSet { get; }
}