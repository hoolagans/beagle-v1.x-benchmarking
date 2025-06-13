using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.Persistence.Entities;

namespace Supermodel.Persistence.Repository;

public interface ILinqDataRepo<TEntity> : IDataRepo<TEntity> where TEntity : class, IEntity, new()
{
    IQueryable<TEntity> Items { get; }
}

public interface IDataRepo<TEntity> : IDataRepo, IDataRepoReadOnly<TEntity>, IDataRepoWriteOnly<TEntity> where TEntity : class, IEntity, new() {}
    
public interface IDataRepo
{
    //[Obsolete("Use Async version instead")]
    //IEntity GetIEntityById(long id);
    Task<IEntity> GetIEntityByIdAsync(long id);

    //[Obsolete("Use Async version instead")]
    //IEntity GetIEntityByIdOrDefault(long id);
    Task<IEntity?> GetIEntityByIdOrDefaultAsync(long id);

    //[Obsolete("Use Async version instead")]
    //List<IEntity> GetIEntityAll();
    Task<IEnumerable<IEntity>> GetIEntityAllAsync();

    void AddIEntity(IEntity item);
    void DeleteIEntity(IEntity item);
}

public interface IDataRepoWriteOnly<in TEntity> where TEntity : class, IEntity, new()
{
    void Add(TEntity item);
    void Delete(TEntity item);
}
    
public interface IDataRepoReadOnly<TEntity> where TEntity : class, IEntity, new()
{
    //[Obsolete("Use Async version instead")]
    //TEntity GetById(long id);
    Task<TEntity> GetByIdAsync(long id);

    //[Obsolete("Use Async version instead")]
    //TEntity GetByIdOrDefault(long id);
    Task<TEntity?> GetByIdOrDefaultAsync(long id);

    //[Obsolete("Use Async version instead")]
    //List<TEntity> GetAll();
    Task<List<TEntity>> GetAllAsync();
}