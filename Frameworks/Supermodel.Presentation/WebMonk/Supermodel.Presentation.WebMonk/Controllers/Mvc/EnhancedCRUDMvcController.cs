using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.Filters;
using WebMonk.Results;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Controllers.Mvc;

public abstract class EnhancedCRUDMvcController<TEntity, TMvcModel, TSearchMvcModel, TMvcView, TDataContext> : EnhancedCRUDMvcController<TEntity, TMvcModel, TMvcModel, TSearchMvcModel, TMvcView, TDataContext>
    where TEntity : class, IEntity, new()
    where TMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new()
    where TMvcView : class, IEnhancedCRUDMvcView<TMvcModel, TMvcModel, TSearchMvcModel>, new();

public abstract class EnhancedCRUDMvcController<TEntity, TDetailMvcModel, TListMvcModel, TSearchMvcModel, TMvcView, TDataContext> : CRUDMvcController<TEntity, TDetailMvcModel, TListMvcModel, TMvcView, TDataContext>
    where TEntity : class, IEntity, new()
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new()
    where TMvcView : class, IEnhancedCRUDMvcView<TDetailMvcModel, TListMvcModel, TSearchMvcModel>, new()
{
    #region Action Methods
    public virtual async Task<ActionResult> GetSearchAsync(int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TSearchMvcModel searchBy;
            try
            {
                searchBy = await GetSearchByUsingTryUpdateAsync(false).ConfigureAwait(false);
            }
            catch (ModelStateInvalidException ex)
            {
                return new TMvcView().RenderSearch((TSearchMvcModel)ex.Model).ToHtmlResult();
            }
            return new TMvcView().RenderSearch(searchBy).ToHtmlResult();
        }
    }

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public virtual async Task<ActionResult> GetListAsync(int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        var isInline = false;

        var modelStateJson = (string?)HttpContext.Current.TempData[Config.ModelState];
        if (modelStateJson != null)
        {
            var modelState = SerializableModelState.CreateFromJson(modelStateJson);
            await modelState.ReplaceInContextAsync().ConfigureAwait(false);
            isInline = true;
        }

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TSearchMvcModel searchBy;
            try
            {
                searchBy = await GetSearchByUsingTryUpdateAsync(isInline).ConfigureAwait(false);
            }
            catch (ModelStateInvalidException ex)
            {
                if (IsGoingToList()) return new TMvcView().RenderList(new ListWithCriteria<TListMvcModel, TSearchMvcModel> { Criteria = (TSearchMvcModel)ex.Model }, 0).ToHtmlResult();
                return new TMvcView().RenderSearch((TSearchMvcModel)ex.Model).ToHtmlResult();
            }

            var items = GetItems();

            items = ApplySearchBy(items, searchBy);
            items = ApplySortBy(items, smSortBy);
            var itemsBeforeSkipAndTake = items; //save items for count
            items = ApplySkipAndTake((IOrderedQueryable<TEntity>)items, smSkip, smTake);

            var entities = await items.ToListAsync().ConfigureAwait(false);
            var mvcModels = new ListWithCriteria<TListMvcModel, TSearchMvcModel> { Criteria = searchBy };
            mvcModels = await mvcModels.MapFromAsync(entities).ConfigureAwait(false);

            //Don't need to do this b/c ReflectionMapper now respects IAsyncInit
            ////Init mvc model if it requires async initialization
            //foreach (var mvcModelItem in mvcModels)
            //{
            //    // ReSharper disable once SuspiciousTypeConversion.Global
            //    if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);
            //}

            var totalCount = await itemsBeforeSkipAndTake.CountAsync().ConfigureAwait(false);
            return new TMvcView().RenderList(mvcModels, totalCount).ToHtmlResult();
        }
    }
    [NonAction] public override Task<ActionResult> GetListAsync()
    {
        return Task.FromResult((ActionResult)new StatusCodeResult(HttpStatusCode.NotFound));    
    }
    #endregion

    #region Protected Helpers
    protected virtual async Task<TSearchMvcModel> GetSearchByUsingTryUpdateAsync(bool isInline)
    {
        var searchBy = new TSearchMvcModel();
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (searchBy is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);
        try
        {
            var valueProviders = await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);
            var queryStringValueProviderOnlyList = valueProviders.Where(x => x.GetType() == typeof(QueryStringValueProvider)).ToList();
            await TryUpdateModelAsync(searchBy, queryStringValueProviderOnlyList).ConfigureAwait(false);

            //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (searchBy is IAsyncValidatableObject validatableObj)
            {
                var vrl = await validatableObj.ValidateAsync(new ValidationContext(validatableObj)).ConfigureAwait(false);
                if (vrl.Count != 0) throw new ValidationResultException(vrl);
            }

            if (!HttpContext.Current.ValidationResultList.IsValid && !isInline) throw new ModelStateInvalidException(searchBy);
            return searchBy;
        }
        catch (ValidationResultException ex)
        {
            HttpContext.Current.ValidationResultList.AddValidationResultList(ex.ValidationResultList);
            throw new ModelStateInvalidException(searchBy);
        }
    }

    protected virtual IQueryable<TEntity> GetPagedSortedAndSearchedItems(int? skip, int? take, string sortBy, TSearchMvcModel searchBy)
    {
        var items = GetItems();
        items = ApplySearchBy(items, searchBy);
        items = ApplySortBy(items, sortBy);
        items = ApplySkipAndTake((IOrderedQueryable<TEntity>)items, skip, take);

        return items;
    }
    protected virtual IQueryable<TEntity> ApplySearchBy(IQueryable<TEntity> items, TSearchMvcModel searchBy)
    {
        return items;
    }

    protected virtual IOrderedQueryable<TEntity> ApplySortBy(IQueryable<TEntity> items, string? sortBy)
    {
        return ControllerCommon.ApplySortBy(items, sortBy);
    }
    protected virtual IQueryable<TEntity> ApplySkipAndTake(IOrderedQueryable<TEntity> orderedItems, int? skip, int? take)
    {
        return ControllerCommon.ApplySkipAndTake(orderedItems, skip, take);
    }

    protected virtual bool IsGoingToList()
    {
        //Is going yo List or Search page. If Search Mvc Model does not validate, we need to know where to go 
        //to show validation errors.
        //Since we cannot rely on the referrer to figure out if we are searching from search or from list,
        //we assume we always search from list. If this is not the case, we expect user to override this method to 
        //provide the logic to determine that.

        return true;
    }
    #endregion
}