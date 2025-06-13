using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.WebMonk.Models.Api;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Api;

public abstract class CRUDApiController<TEntity, TApiModel, TDataContext> : CRUDApiController<TEntity, TApiModel, TApiModel, TDataContext>
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TApiModel : ApiModelForEntity<TEntity>, new();

public abstract class CRUDApiController<TEntity, TDetailApiModel, TListApiModel, TDataContext> : ApiControllerBase
    where TDataContext : class, IDataContext, new()
    where TEntity : class, IEntity, new()
    where TDetailApiModel : ApiModelForEntity<TEntity>, new()
    where TListApiModel : ApiModelForEntity<TEntity>, new()
{
    #region Action Methods
    public virtual async Task<ActionResult> GetAllAsync(int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            var entities = await GetPagedAndSortedItems(smSkip, smTake, smSortBy).ToListAsync().ConfigureAwait(false);
            var apiModels = new List<TListApiModel>();
            apiModels = await apiModels.MapFromAsync(entities).ConfigureAwait(false);
            return new JsonApiResult(apiModels);
        }
    }
    public virtual async Task<ActionResult> GetCountAllAsync(int? smSkip = null, int? smTake = null)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            return new JsonApiResult(await GetPagedAndSortedItems(smSkip, smTake, null).CountAsync().ConfigureAwait(false));
        }
    }
    public virtual async Task<ActionResult> GetAsync(long id)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            if (id == 0)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult(await new TDetailApiModel().MapFromAsync(new TEntity()).ConfigureAwait(false));
            }

            var entityItem = await GetItemOrDefaultAsync(id).ConfigureAwait(false);
            if (entityItem == null)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult($"Entity with id={id} does not exist", HttpStatusCode.NotFound);
            }
            var apiModelItem = await new TDetailApiModel().MapFromAsync(entityItem).ConfigureAwait(false);

            return new JsonApiResult(apiModelItem);
        }
    }
    public virtual async Task<ActionResult> DeleteAsync(long id)
    {
        await using (var uow = new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            TEntity? entityItem;
            try
            {
                entityItem = await GetItemOrDefaultAsync(id).ConfigureAwait(false);
                if (entityItem == null)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return new JsonApiResult($"Entity with id={id} does not exist", HttpStatusCode.NotFound);
                }
                entityItem.Delete();
            }
            catch (UnableToDeleteException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                return new JsonApiResult(ex.Message, HttpStatusCode.Conflict);
            }
            catch (Exception ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                return new JsonApiResult(ex.Message, HttpStatusCode.InternalServerError);
            }

            await AfterDeleteAsync(id, entityItem).ConfigureAwait(false);
                
            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync().ConfigureAwait(false);
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync().ConfigureAwait(false);

            return new StatusCodeResult(HttpStatusCode.NoContent);
        }
    }
    public virtual async Task<ActionResult> PutAsync(long id, TDetailApiModel apiModelItem)
    {
        await using (var uow = new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            if (id == 0)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult("ApiCRUDController.Put: id == 0", HttpStatusCode.BadRequest);
            }

            var entityItem = await GetItemOrDefaultAndCacheItAsync(id).ConfigureAwait(false);
            if (entityItem == null)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult($"Entity with id={id} does not exist", HttpStatusCode.NotFound);
            }
            try
            {
                entityItem = await TryUpdateEntityAsync(entityItem, apiModelItem).ConfigureAwait(false);
                if (id != apiModelItem.Id)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return new JsonApiResult("ApiCRUDController.Put: Id in Url must match Id in Json", HttpStatusCode.BadRequest);
                }
            }
            catch (ModelStateInvalidException)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult(await new ValidationErrorsApiModel().MapFromAsync(HttpContext.Current.ValidationResultList).ConfigureAwait(false), HttpStatusCode.ExpectationFailed);
            }

            await AfterUpdateAsync(id, entityItem, apiModelItem).ConfigureAwait(false);

            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync().ConfigureAwait(false);
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync().ConfigureAwait(false);

            return new StatusCodeResult(HttpStatusCode.OK);
        }
    }
    public virtual async Task<ActionResult> PostAsync(TDetailApiModel apiModelItem)
    {
        await using (var uow =new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.Yes))
        {
            var entityItem = new TEntity();
            try
            {
                entityItem = await TryUpdateEntityAsync(entityItem, apiModelItem).ConfigureAwait(false);
                if (apiModelItem.Id != 0)
                {
                    UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                    return new JsonApiResult("ApiCRUDController.Post: Id in Json must be 0 or be blank", HttpStatusCode.BadRequest);
                }
            }
            catch (ModelStateInvalidException)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false;
                return new JsonApiResult(await new ValidationErrorsApiModel().MapFromAsync(HttpContext.Current.ValidationResultList).ConfigureAwait(false), HttpStatusCode.ExpectationFailed);
            }
            entityItem.Add();

            await AfterCreateAsync(entityItem, apiModelItem).ConfigureAwait(false);
                
            if (ReferenceEquals(uow.DataContext, UnitOfWorkContext<TDataContext>.CurrentDataContext)) await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync().ConfigureAwait(false);
            else await UnitOfWorkContext<TDataContext>.CurrentDataContext.SaveChangesAsync().ConfigureAwait(false);

            return new JsonApiResult(entityItem.Id);
        }
    }
    #endregion

    #region Overrides
    protected override async Task<(bool, object?[])> TryBindAndValidateParametersAsync(MethodInfo actionMethodInfo)
    {
        await using (new UnitOfWorkIfNoAmbientContext<TDataContext>(MustBeWritable.No))
        {
            return await base.TryBindAndValidateParametersAsync(actionMethodInfo).ConfigureAwait(false);
        }
    }
    #endregion

    #region Protected helpers
    protected virtual async Task<TEntity?> GetItemOrDefaultAsync(long id)
    {
        return await GetItems().SingleOrDefaultAsync(x => x.Id == id).ConfigureAwait(false);
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
            var vr = await apiModelItem.ValidateAsync(new ValidationContext(apiModelItem)).ConfigureAwait(false);
            if (vr.Any()) throw new ValidationResultException(vr);
                
            entityItem = await apiModelItem.MapToAsync(entityItem).ConfigureAwait(false);
            if (!HttpContext.Current.ValidationResultList.IsValid) throw new ModelStateInvalidException(apiModelItem);
            return entityItem;
        }
        catch (ValidationResultException ex)
        {
            HttpContext.Current.ValidationResultList.AddValidationResultList(ex.ValidationResultList);
            throw new ModelStateInvalidException(apiModelItem);
        }
    }
    #endregion
}