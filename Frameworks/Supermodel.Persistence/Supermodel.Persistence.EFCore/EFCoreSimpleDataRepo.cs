using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;

namespace Supermodel.Persistence.EFCore;

public class EFCoreSimpleDataRepo<TEntity> : IEFCoreDataRepo<TEntity> where TEntity : class, IEntity, new()
{
    #region Methods
    public virtual void Add(TEntity item)
    {
        if (item.Id == 0) DbSet.Add(item);
    }
    public virtual void AddIEntity(IEntity item)
    {
        Add((TEntity)item);
    }

    public virtual void Delete(TEntity item)
    {
        DbSet.Remove(item);
    }
    public virtual void DeleteIEntity(IEntity item)
    {
        Delete((TEntity)item);
    }

    //public virtual TEntity GetById(long id)
    //{
    //    var item = GetByIdOrDefault(id);
    //    if (item == null) throw new SupermodelException($"{nameof(GetById)} can't find an entity");
    //    return item;
    //}
    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        var item =await GetByIdOrDefaultAsync(id);
        if (item == null) throw new SupermodelException($"{nameof(GetByIdAsync)} can't find an entity");
        return item;
    }

    //public virtual IEntity GetIEntityById(long id)
    //{
    //    return GetById(id);
    //}
    public virtual async Task<IEntity> GetIEntityByIdAsync(long id)
    {
        return await GetByIdAsync(id);
    }
        
    //public virtual TEntity GetByIdOrDefault(long id)
    //{
    //    return Items.SingleOrDefault(p => p.Id == id);
    //}
    public virtual async Task<TEntity?> GetByIdOrDefaultAsync(long id)
    {
        return await Items.SingleOrDefaultAsync(p => p.Id == id);
    }

    //public virtual IEntity GetIEntityByIdOrDefault(long id)
    //{
    //    return GetByIdOrDefault(id);
    //}
    public virtual async Task<IEntity?> GetIEntityByIdOrDefaultAsync(long id)
    {
        return await GetByIdOrDefaultAsync(id);
    }
        
    //public virtual List<TEntity> GetAll()
    //{
    //    return Items.ToList();
    //}
    public virtual Task<List<TEntity>> GetAllAsync()
    {
        return Items.ToListAsync();
    }

    //public virtual List<IEntity> GetIEntityAll()
    //{
    //    return (List<IEntity>)(IEnumerable<IEntity>)GetAll();
    //}
    public virtual async Task<IEnumerable<IEntity>> GetIEntityAllAsync()
    {
        return await GetAllAsync();
    }
    #endregion

    #region Properties
    public virtual DbSet<TEntity> DbSet => (DbSet<TEntity>)UnitOfWorkContextCore.CurrentDataContext.Items<TEntity>();
    public virtual IQueryable<TEntity> Items
    {
        get
        {
            //this should improve performance if LoadReadOnlyEntitiesAsNoTracking is set
            var context = (EFCoreDataContext)UnitOfWorkContextCore.CurrentDataContext;
            if (context.LoadReadOnlyEntitiesAsNoTracking && context.IsReadOnly) return DbSet.AsNoTracking();

            return DbSet;
        }
    }
    #endregion
}