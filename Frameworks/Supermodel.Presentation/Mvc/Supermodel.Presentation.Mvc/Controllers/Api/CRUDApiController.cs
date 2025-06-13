using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Mvc.Models.Api;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Controllers.Api;

public abstract class CRUDApiController<TEntity, TApiModel, TDataContext> : CRUDApiController<TEntity, TApiModel, TApiModel, TDataContext>
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TApiModel : ApiModelForEntity<TEntity>, new()
{ }

public abstract class CRUDApiController<TEntity, TDetailApiModel, TListApiModel, TDataContext> : ApiControllerBase
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TDetailApiModel : ApiModelForEntity<TEntity>, new()
    where TListApiModel : ApiModelForEntity<TEntity>, new()
{
    #region Action Methods
    [HttpGet]
    public virtual async Task<IActionResult> All(int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var entities = await GetPagedAndSortedItems(smSkip, smTake, smSortBy).ToListAsync();
            var apiModels = new List<TListApiModel>();
            apiModels = await apiModels.MapFromAsync(entities);
            return StatusCode((int)HttpStatusCode.OK, apiModels);
        }
    }

    [HttpGet]
    public virtual async Task<IActionResult> CountAll(int? smSkip = null, int? smTake = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            return StatusCode((int)HttpStatusCode.OK, await GetPagedAndSortedItems(smSkip, smTake, null).CountAsync());
        }
    }

    [HttpGet("{id:long}")]
    public virtual async Task<IActionResult> Get(long id)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            if (id == 0)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.OK, await new TDetailApiModel().MapFromAsync(new TEntity()));
            }

            var entityItem = await GetItemOrDefaultAsync(id);
            if (entityItem == null)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.NotFound, $"Entity with id={id} does not exist");
            }
            var apiModelItem = await new TDetailApiModel().MapFromAsync(entityItem);

            return StatusCode((int)HttpStatusCode.OK, apiModelItem);
        }
    }

    [HttpDelete("{id:long}")]
    public virtual async Task<IActionResult> Delete(long id)
    {
        await using (var uow = new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            TEntity? entityItem;
            try
            {
                entityItem = await GetItemOrDefaultAsync(id);
                if (entityItem == null)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return StatusCode((int)HttpStatusCode.NotFound, $"Entity with id={id} does not exist");
                }
                entityItem.Delete();
            }
            catch (UnableToDeleteException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                return StatusCode((int)HttpStatusCode.Conflict, ex.Message);
            }
            catch (Exception ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

            await AfterDeleteAsync(id, entityItem);
                
            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync();
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }

    [HttpPut("{id:long}")]
    public virtual async Task<IActionResult> Put(long id, TDetailApiModel apiModelItem)
    {
        await using (var uow = new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            if (id == 0)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.BadRequest, "ApiCRUDController.Put: id == 0");
            }

            var entityItem = await GetItemOrDefaultAndCacheItAsync(id);
            if (entityItem == null)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.NotFound, $"Entity with id={id} does not exist");
            }
            try
            {
                entityItem = await TryUpdateEntityAsync(entityItem, apiModelItem);
                if (id != apiModelItem.Id)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return StatusCode((int)HttpStatusCode.BadRequest, "ApiCRUDController.Put: Id in Url must match Id in Json");
                }
            }
            catch (ModelStateInvalidException)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.ExpectationFailed, await new ValidationErrorsApiModel().MapFromAsync(ModelState));
            }

            await AfterUpdateAsync(id, entityItem, apiModelItem);

            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync();
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.OK);
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> Post(TDetailApiModel apiModelItem)
    {
        await using (var uow =new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            var entityItem = new TEntity();
            try
            {
                entityItem = await TryUpdateEntityAsync(entityItem, apiModelItem);
                if (apiModelItem.Id != 0)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return StatusCode((int)HttpStatusCode.BadRequest, "ApiCRUDController.Post: Id in Json must be 0 or be blank");
                }
            }
            catch (ModelStateInvalidException)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return StatusCode((int)HttpStatusCode.ExpectationFailed, await new ValidationErrorsApiModel().MapFromAsync(ModelState));
            }
            entityItem.Add();

            await AfterCreateAsync(entityItem, apiModelItem);
                
            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync();
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.OK, entityItem.Id);
        }
    }
    #endregion

    #region Protected helpers
    protected virtual Task<TEntity?> GetItemOrDefaultAsync(long id)
    {
        return GetItems().SingleOrDefaultAsync(x => x.Id == id);
    }
    protected virtual async Task<TEntity?> GetItemOrDefaultAndCacheItAsync(long id)
    {
        var item = await GetItemOrDefaultAsync(id);
        UnitOfWorkContext.CustomValues[$"Item_{id}"] = item; //we cache this, for MvcModel validation
        return item;
    }
    protected virtual IQueryable<TEntity> GetItems()
    {
        return ControllerCommon.GetItems<TEntity>();
    }
    protected virtual IQueryable<TEntity> GetPagedAndSortedItems(int? skip, int? take, string? sortBy)
    {
        var items = GetItems();
            
        if (!string.IsNullOrEmpty(sortBy)) items = ApplySortBy(items, sortBy);
            
        if (skip != null || take != 0)
        {
            //if we did not sort by anything, sort by Id because we need it sorted for skip and take
            if (!(items is IOrderedQueryable<TEntity>)) items = items.OrderBy(x => x.Id);

            items = ApplySkipAndTake((IOrderedQueryable<TEntity>)items, skip, take);
        }

        return items;
    }

    protected virtual IQueryable<TEntity> ApplySkipAndTake(IOrderedQueryable<TEntity> orderedItems, int? skip, int? take)
    {
        return ControllerCommon.ApplySkipAndTake(orderedItems, skip, take);
    }
    protected virtual IOrderedQueryable<TEntity> ApplySortBy(IQueryable<TEntity> items, string? sortBy)
    {
        return ControllerCommon.ApplySortBy(items, sortBy);
    }

    protected virtual Task AfterDeleteAsync(long id, TEntity entityItem)
    {
        //Do nothing
        return Task.CompletedTask;
    }
    protected virtual Task AfterUpdateAsync(long id, TEntity entityItem, TDetailApiModel apiModelItem)
    {
        //Do nothing
        return Task.CompletedTask;
    }
    protected virtual Task AfterCreateAsync(TEntity entityItem, TDetailApiModel apiModelItem)
    {
        //Do nothing
        return Task.CompletedTask;
    }
    protected virtual async Task<TEntity> TryUpdateEntityAsync(TEntity entityItem, TDetailApiModel apiModelItem)
    {
        try
        {
            //Validate apiModelItem here
            var vr = await apiModelItem.ValidateAsync(new ValidationContext(apiModelItem));
            if (vr.Any()) throw new ValidationResultException(vr);
                
            entityItem = await apiModelItem.MapToAsync(entityItem);
            if (!ModelState.IsValid) throw new ModelStateInvalidException(apiModelItem);
            return entityItem;
        }
        catch (ValidationResultException ex)
        {
            ModelState.AddValidationResultList(ex.ValidationResultList);
            throw new ModelStateInvalidException(apiModelItem);
        }
    }
    #endregion
}