using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.Mvc.Extensions;
using Supermodel.Presentation.Mvc.Extensions.Gateway;
using Supermodel.Presentation.Mvc.Models;
using Supermodel.Presentation.Mvc.Models.Mvc;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Controllers.Mvc;

public abstract class ChildCRUDController<TChildEntity, TChildDetailMvcModel, TParentEntity, TParentController, TDataContext> : Controller
    where TChildEntity : class, IEntity, new()
    where TParentEntity : class, IEntity, new()
    where TChildDetailMvcModel : class, IChildMvcModelForEntity<TChildEntity, TParentEntity>, new()
    where TParentController : Controller
    where TDataContext : class, IDataContext, new()
{
    #region Action Methods
    public virtual Task<IActionResult> List(long parentId)
    {
        return Task.FromResult(GoToParentDetail(parentId));
    }

    [HttpGet]
    public virtual async Task<IActionResult> Detail(long id, long? parentId, HttpGet ignore)
    {
        var modelStateJson = TempData[Config.ModelState];
        if (modelStateJson != null) ModelState.Merge(ControllerCommon.DeserializeModelState((string)modelStateJson));

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TChildDetailMvcModel mvcModelItem;

            if (id == 0)
            {
                if (parentId == null) throw new SupermodelException("parentId == null when id == 0");
                mvcModelItem = new TChildDetailMvcModel { ParentId = parentId }; //We set parentID twice, in case we may need it during MapFromObject
                    
                //Init mvc model is it requires async initialization
                if (mvcModelItem is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync();
                    
                mvcModelItem = await mvcModelItem.MapFromAsync(new TChildEntity());
                mvcModelItem.ParentId = parentId;

                return View(mvcModelItem);
            }

            var entityItem = await GetItemAsync(id);
            mvcModelItem = new TChildDetailMvcModel();
                
            //Init mvc model is it requires async initialization
            if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync();
                
            mvcModelItem = await mvcModelItem.MapFromAsync(entityItem);

            return View(mvcModelItem);
        }
    }

    [HttpPut]
    public virtual async Task<IActionResult> Detail(long id, bool? isInline, HttpPut ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id == 0) throw new SupermodelException("CRUDControllerBase.Detail[Post]: id == 0");

            var entityItem = await GetItemAndCacheItAsync(id);
            TChildDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix, null);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                    
                //Init ex.Model is it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync();

                if (isInline == true)
                {
                    TempData[Config.ModelState] = ControllerCommon.SerializeModelState(ModelState);
                    return GoToListScreen(new TChildDetailMvcModel().GetParentEntity(entityItem)!.Id, id);
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
    public virtual async Task<IActionResult> Detail(long id, long parentId, bool? isInline, HttpPost ignore)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id != 0) throw new SupermodelException("CRUDControllerBase.Detail[Put]: id != 0");

            var entityItem = new TChildEntity();
            TChildDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix, parentId);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                    
                //Init ex.Model is it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync();

                if (isInline == true)
                {
                    TempData[Config.ModelState] = ControllerCommon.SerializeModelState(ModelState);
                    return GoToListScreen(parentId, id);
                }
                else
                {
                    return View(ex.Model);
                }
            }
            entityItem.Add();
            return await AfterCreateAsync(id, parentId, entityItem, mvcModelItem);
        }
    }

    [HttpDelete]
    public virtual async Task<IActionResult> Detail(long id, HttpDelete ignore)
    {
        TChildEntity? entityItem = null;
        await using (new UnitOfWork<TDataContext>())
        {
            long? parentId = null;
            try
            {
                entityItem = await GetItemAsync(id);
                parentId = new TChildDetailMvcModel().GetParentEntity(entityItem)!.Id;
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
            if (parentId == null) throw new SupermodelException("Unknown parentId");
            return await AfterDeleteAsync(id, parentId.Value, entityItem!);
        }
    }

    [HttpGet]
    public virtual async Task<IActionResult> BinaryFile(long id, string pn, HttpGet ignore)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(await GetItemAsync(id));

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
            var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(entityItem);

            //see if pn is a required property
            var propInfo = typeof(TChildDetailMvcModel).GetProperty(pn) ?? throw new SupermodelException("GetProperty(pn) == null");
            if (Attribute.GetCustomAttribute(propInfo, typeof(RequiredAttribute), true) != null)
            {
                TempData.Super().NextPageModalMessage = "Cannot delete required field";
                var routeValues = new RouteValueDictionary(ControllerContext.RouteData.Values) { ["Action"] = "Detail" };
                return RedirectToRoute(routeValues);
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
    protected virtual Task<IActionResult> AfterUpdateAsync(long id, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen(mvcModelItem.GetParentEntity(entityItem)!.Id));
    }
    protected virtual Task<IActionResult> AfterCreateAsync(long id, long parentId, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen(mvcModelItem.GetParentEntity(entityItem)!.Id));
    }
    protected virtual Task<IActionResult> AfterDeleteAsync(long id, long parentId, TChildEntity entityItem)
    {
        return Task.FromResult(GoToListScreen(parentId));
    }
    protected virtual Task<IActionResult> AfterBinaryDeleteAsync(long id, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(StayOnDetailScreen(id));
    }

    protected virtual IQueryable<TChildEntity> GetItems()
    {
        return LinqRepoFactory.Create<TChildEntity>().Items;
    }
    protected virtual async Task<TChildEntity> GetItemAndCacheItAsync(long id)
    {
        var item = await GetItemAsync(id);
        UnitOfWorkContext.CustomValues[$"Item_{id}"] = item; //we cache this, for MvcModel validation
        return item;
    }
    protected virtual Task<TChildEntity> GetItemAsync(long id)
    {
        return GetItems().SingleAsync(x => x.Id == id);
    }

    protected virtual bool SuggestOpenBinaryFilesInline => false;

    //this method will catch validation exceptions that happen during mapping from mvc to domain (when it runs validation for mvc model by creating a domain object)
    protected virtual async Task<Tuple<TChildEntity, TChildDetailMvcModel>> TryUpdateEntityAsync(TChildEntity entityItem, string prefix, long? parentId)
    {
        var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(entityItem);
        if (parentId != null) mvcModelItem.ParentId = parentId;
        try
        {
            await TryUpdateModelAsync(mvcModelItem, prefix);
            if (!ModelState.IsValid) throw new ModelStateInvalidException(mvcModelItem);

            entityItem = await mvcModelItem.MapToAsync(entityItem);
            if (!ModelState.IsValid) throw new ModelStateInvalidException(mvcModelItem);

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

    protected virtual IActionResult GoToListScreen(long parentId, long? selectedId = null)
    {
        var routeValues = HttpContext.Request.Query.ToRouteValueDictionary();
        routeValues.AddOrUpdateWith("parentId", parentId);
        routeValues.AddOrUpdateWith("selectedId", selectedId);
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction("List", routeValues);
    }
    protected virtual IActionResult StayOnDetailScreen(long id)
    {
        var routeValues = HttpContext.Request.Query.ToRouteValueDictionary();
        routeValues.Add("id", id);
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction("Detail", routeValues);
    }
    protected virtual IActionResult GoToParentDetail(long parentId)
    {
        var routeValues = HttpContext.Request.Query.ToRouteValueDictionary();
        routeValues.Remove("parentId");
        routeValues.AddOrUpdateWith("id", parentId);
        return RedirectToAction("Detail", typeof(TParentController).GetControllerName(), routeValues);
    }
    #endregion
}