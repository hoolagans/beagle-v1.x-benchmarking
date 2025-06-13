using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Mvc.Models.Mvc;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Controllers.Mvc;

public abstract class EnhancedCRUDController<TEntity, TMvcModel, TSearchMvcModel, TDataContext> : EnhancedCRUDController<TEntity, TMvcModel, TMvcModel, TSearchMvcModel, TDataContext>
    where TEntity : class, IEntity, new()
    where TMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new()
{ }

public abstract class EnhancedCRUDController<TEntity, TDetailMvcModel, TListMvcModel, TSearchMvcModel, TDataContext> : CRUDController<TEntity, TDetailMvcModel, TListMvcModel, TDataContext>
    where TEntity : class, IEntity, new()
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new()
{
    #region Action Methods
    public virtual async Task<IActionResult> Search(int? smTake = null, string? smSortBy = null)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TSearchMvcModel searchBy;
            try
            {
                searchBy = await GetSearchByUsingTryUpdateAsync(false);
            }
            catch (ModelStateInvalidException ex)
            {
                return View("Search", ex.Model);
            }
            return View(searchBy);
        }
    }

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public virtual async Task<IActionResult> List(int? smSkip = null, int? smTake = null, string? smSortBy = null)
    {
        var isInline = false;

        var modelStateJson = TempData[Config.ModelState];
        if (modelStateJson != null)
        {
            ModelState.Merge(ControllerCommon.DeserializeModelState((string)modelStateJson));
            isInline = true;
        }

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TSearchMvcModel searchBy;
            try
            {
                searchBy = await GetSearchByUsingTryUpdateAsync(isInline);
            }
            catch (ModelStateInvalidException ex)
            {
                if (IsGoingToList())
                {
                    SetUpPagingViewBag(0);

                    return View("List", new ListWithCriteria<TListMvcModel, TSearchMvcModel> { Criteria = (TSearchMvcModel)ex.Model });
                }
                return View("Search", ex.Model);
            }

            var items = GetItems();

            items = ApplySearchBy(items, searchBy);
            items = ApplySortBy(items, smSortBy);
            var itemsBeforeSkipAndTake = items; //save items for count
            items = ApplySkipAndTake((IOrderedQueryable<TEntity>)items, smSkip, smTake);

            var entities = await items.ToListAsync();
            var mvcModels = new ListWithCriteria<TListMvcModel, TSearchMvcModel> { Criteria = searchBy };
            mvcModels = await mvcModels.MapFromAsync(entities);

            //Don't need to do this b/c ReflectionMapper now respects IAsyncInit
            ////Init mvc model if it requires async initialization
            //foreach (var mvcModelItem in mvcModels)
            //{
            //    if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
            //}

            SetUpPagingViewBag(await itemsBeforeSkipAndTake.CountAsync());

            return View("List", mvcModels);
        }
    }
        
    [NonAction]
    public override Task<IActionResult> List()
    {
        return Task.FromResult((IActionResult)NotFound());
    }
    #endregion

    #region Protected Helpers
    protected virtual async Task<TSearchMvcModel> GetSearchByUsingTryUpdateAsync(bool isInline)
    {
        var searchBy = new TSearchMvcModel();
        if (searchBy is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
        try
        {
            var queryStringValueProvider = new QueryStringValueProvider(BindingSource.Query, Request.Query, CultureInfo.CurrentCulture);
            await TryUpdateModelAsync(searchBy, "", queryStringValueProvider);

            //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
            if (searchBy is IAsyncValidatableObject validatableObj)
            {
                var vrl = await validatableObj.ValidateAsync(new ValidationContext(validatableObj));
                if (vrl.Count != 0) throw new ValidationResultException(vrl);
            }

            if (!ModelState.IsValid && !isInline) throw new ModelStateInvalidException(searchBy);
            return searchBy;
        }
        catch (ValidationResultException ex)
        {
            ModelState.AddValidationResultList(ex.ValidationResultList);
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

    protected virtual void SetUpPagingViewBag(int totalCount)
    {
        ViewBag.SupermodelTotalCount = totalCount;
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