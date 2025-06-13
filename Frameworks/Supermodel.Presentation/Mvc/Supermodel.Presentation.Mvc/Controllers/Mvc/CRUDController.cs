using Microsoft.AspNetCore.Mvc;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Supermodel.Presentation.Mvc.Extensions.Gateway;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.ReflectionMapper;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence;
using Supermodel.Presentation.Mvc.Models;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Controllers.Mvc;

public abstract class CRUDController<TEntity, TMvcModel, TDataContext> : CRUDController<TEntity, TMvcModel, TMvcModel, TDataContext>
    where TEntity : class, IEntity, new()
    where TMvcModel : class, IMvcModelForEntity, new()
    where TDataContext : class, IDataContext, new()
{ }

public abstract class CRUDController<TEntity, TDetailMvcModel, TListMvcModel, TDataContext> : Controller
    where TEntity : class, IEntity, new()
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TDataContext : class, IDataContext, new()
{
    #region Action Methods
    public virtual async Task<IActionResult> List()
    {
        var modelStateJson = TempData[Config.ModelState];
        if (modelStateJson != null) ModelState.Merge(ControllerCommon.DeserializeModelState((string)modelStateJson));

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            //await SetUpNewItemMvcModelAsync();
                
            var entities = await GetItems().ToListAsync();
            var mvcModels = new List<TListMvcModel>();
            mvcModels = await mvcModels.MapFromAsync(entities);
            //mvcModels = mvcModels.OrderBy(p => p.Label).ToList();

            //Init mvc model if it requires async initialization
            foreach (var mvcModelItem in mvcModels)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
            }

            return View(mvcModels);
        }
    }

    [HttpGet]
    public virtual async Task<IActionResult> Detail(long id, HttpGet ignore)
    {
        var modelStateJson = TempData[Config.ModelState];
        if (modelStateJson != null) ModelState.Merge(ControllerCommon.DeserializeModelState((string)modelStateJson)); 
            
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var mvcModelItem = new TDetailMvcModel();

            //Init mvc model if it requires async initialization
            if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();

            if (id == 0) 
            {
                mvcModelItem = await mvcModelItem.MapFromAsync(new TEntity());
            }
            else
            {
                var entityItem = await GetItemAsync(id);
                mvcModelItem = await mvcModelItem.MapFromAsync(entityItem);
            }
                
            return View(mvcModelItem);
        }
    }

    [HttpPut]
    public virtual async Task<IActionResult> Detail(long id, bool? isInline, HttpPut ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id == 0) throw new SupermodelException("MvcCRUDController.Detail[Put]: id == 0");

            var entityItem = await GetItemAndCacheItAsync(id);
            TDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                    
                //Init ex.Model is it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync();
                    
                if (isInline == true)
                {
                    TempData[Config.ModelState] = ControllerCommon.SerializeModelState(ModelState);
                    return GoToListScreen(id);
                }
                else
                {
                    return View(ex.Model);
                }
            }

            return await AfterUpdateAsync(id, entityItem, mvcModelItem);
        }
    }

    [HttpPost]
    public virtual async Task<IActionResult> Detail(long id, bool? isInline, HttpPost ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id != 0) throw new SupermodelException("CRUDControllerBase.Detail[Post]: id != 0");

            var entityItem = new TEntity();
            TDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction

                //Init ex.Model fs it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync();

                if (isInline == true)
                {
                    TempData[Config.ModelState] = ControllerCommon.SerializeModelState(ModelState);
                    return GoToListScreen(id);
                }
                else
                {
                    return View(ex.Model);
                }
            }
            entityItem.Add();

            return await AfterCreateAsync(id, entityItem, mvcModelItem);
        }
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Detail(long id, HttpDelete ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            TEntity? entityItem = null;
            try
            {
                entityItem = await GetItemAsync(id);
                entityItem.Delete();
            }
            catch (UnableToDeleteException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                TempData.Super().NextPageModalMessage = ex.Message;
            }
            catch (Exception)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                TempData.Super().NextPageModalMessage = "PROBLEM!!!\\n\\nUnable to delete. Most likely reason: references from other entities.";
            }

            if (entityItem == null) throw new SupermodelException("MvcCRUDController.Detail[Delete]: entityItem == null: this should never happen");
            return await AfterDeleteAsync(id, entityItem);
        }
    }

    [HttpGet]
    public virtual async Task<IActionResult> BinaryFile(long id, string pn, HttpGet ignore)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var mvcModelItem = await new TDetailMvcModel().MapFromAsync(await GetItemAsync(id));

            var file = (BinaryFileModelBase?)mvcModelItem.PropertyGet(pn);
            if (file == null || file.IsEmpty) return new StatusCodeResult((int)HttpStatusCode.NotFound);

            if (SuggestOpenBinaryFilesInline) Response.Headers.Append("Content-Disposition", "inline; filename=" + file.FileName);
            else Response.Headers.Append("Content-Disposition", "attachment; filename=" + file.FileName);

            var contentType = MimeTypes.GetMimeType(file.FileName);
            return File(file.BinaryContent, contentType);
        }
    }

    [HttpDelete]
    public virtual async Task<IActionResult> BinaryFile(long id, string pn, HttpDelete ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            var entityItem = await GetItemAsync(id);
            var mvcModelItem = await new TDetailMvcModel().MapFromAsync(entityItem);

            //see if pn is a required property
            var propInfo = typeof(TDetailMvcModel).GetProperty(pn) ?? throw new SupermodelException("GetProperty(pn) == null");
            if (Attribute.GetCustomAttribute(propInfo, typeof(RequiredAttribute), true) != null)
            {
                TempData.Super().NextPageModalMessage = "Cannot delete required field";
                return StayOnDetailScreen(id);
            }

            var file = (BinaryFileModelBase?)mvcModelItem.PropertyGet(pn);
            if (file == null) throw new SystemException("file == null");

            file.Empty();
            entityItem = await mvcModelItem.MapToAsync(entityItem);

            var result = await AfterBinaryDeleteAsync(id, entityItem, mvcModelItem);
            await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync();
            return result;
        }
    }
    #endregion

    #region Protected Methods & Properties
    protected virtual Task<IActionResult> AfterDeleteAsync(long id, TEntity entityItem)
    {
        return Task.FromResult(GoToListScreen());
    }
    protected virtual Task<IActionResult> AfterUpdateAsync(long id, TEntity entityItem, TDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen());
    }
    protected virtual Task<IActionResult> AfterCreateAsync(long id, TEntity entityItem, TDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen());
    }
    protected virtual Task<IActionResult> AfterBinaryDeleteAsync(long id, TEntity entityItem, TDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(StayOnDetailScreen(id));
        //return Task.FromResult(GoToListScreen());
    }

    protected virtual Task<TEntity> GetItemAsync(long id)
    {
        return GetItems().SingleAsync(x => x.Id == id);
    }
    protected virtual async Task<TEntity> GetItemAndCacheItAsync(long id)
    {
        var item = await GetItemAsync(id);
        UnitOfWorkContext.CustomValues[$"Item_{id}"] = item; //we cache this, for MvcModel validation
        return item;
    }
    protected virtual IQueryable<TEntity> GetItems()
    {
        return ControllerCommon.GetItems<TEntity>();
    }

    protected virtual bool SuggestOpenBinaryFilesInline => false;

    protected virtual Task<IActionResult> HandleInlineEditValidationErrorsAsync(long id)
    {
        throw new SupermodelException("You must Use EnhancedCRUDController for inline editing.");
    }

    //this methods will catch validation exceptions that happen during mapping from mvc to domain (when it runs validation for mvc model by creating a domain object)
    protected virtual async Task<Tuple<TEntity, TDetailMvcModel>> TryUpdateEntityAsync(TEntity entityItem, string prefix)
    {
        var mvcModelItem = new TDetailMvcModel();
        if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
        mvcModelItem = await mvcModelItem.MapFromAsync(entityItem);

        try
        {
            await TryUpdateModelAsync(mvcModelItem, prefix);
            if (ModelState.IsValid != true) throw new ModelStateInvalidException(mvcModelItem);

            entityItem = await mvcModelItem.MapToAsync(entityItem);
            if (ModelState.IsValid != true) throw new ModelStateInvalidException(mvcModelItem);

            //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
            var vrl = await mvcModelItem.ValidateAsync(new ValidationContext(mvcModelItem));
            if (vrl.Count != 0) throw new ValidationResultException(vrl);

            return Tuple.Create(entityItem, mvcModelItem);
        }
        catch (ValidationResultException ex)
        {
            ModelState.AddValidationResultList(ex.ValidationResultList, prefix);
            throw new ModelStateInvalidException(mvcModelItem);
        }
    }

    protected virtual IActionResult GoToListScreen(long? selectedId = null)
    {
        var routeValues = HttpContext.Request.Query.ToRouteValueDictionary();
        routeValues.AddOrUpdateWith("selectedId", selectedId);
        routeValues.Remove("pn");
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction(nameof(List), routeValues);
    }
    protected virtual IActionResult StayOnDetailScreen(long id)
    {
        var routeValues = HttpContext.Request.Query.ToRouteValueDictionary();
        routeValues.Add("id", id);
        routeValues.Remove("pn");
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction(nameof(Detail), routeValues);
    }
    #endregion
}