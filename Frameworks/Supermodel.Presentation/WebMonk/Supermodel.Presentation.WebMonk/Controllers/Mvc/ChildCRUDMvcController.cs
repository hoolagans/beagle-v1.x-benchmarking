using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Persistence;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.WebMonk.Extensions.Gateway;
using Supermodel.Presentation.WebMonk.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using Supermodel.ReflectionMapper;
using WebMonk;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.HttpRequestHandlers.Controllers;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Mvc;

public abstract class ChildCRUDMvcController<TChildEntity, TChildDetailMvcModel, TParentEntity, TParentController, TMvcView, TDataContext> : MvcController
    where TChildEntity : class, IEntity, new()
    where TParentEntity : class, IEntity, new()
    where TChildDetailMvcModel : class, IChildMvcModelForEntity<TChildEntity, TParentEntity>, new()
    where TParentController : MvcController
    where TMvcView : class, IChildCRUDMvcView<TChildDetailMvcModel>, new()
    where TDataContext : class, IDataContext, new()
{
    #region Action Methods
    public virtual Task<ActionResult> GetListAsync(long parentId)
    {
        return Task.FromResult(GoToParentDetail(parentId));
    }
    public virtual async Task<ActionResult> GetDetailAsync(long id, long? parentId = null)
    {
        var modelStateJson = (string?)HttpContext.Current.TempData[Config.ModelState];
        if (modelStateJson != null)
        {
            var modelState = SerializableModelState.CreateFromJson(modelStateJson);
            await modelState.ReplaceInContextAsync().ConfigureAwait(false);
        }

        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            TChildDetailMvcModel mvcModelItem;

            if (id == 0)
            {
                if (parentId == null) throw new SupermodelException("parentId == null when id == 0");
                mvcModelItem = new TChildDetailMvcModel { ParentId = parentId }; //We set parentID twice, in case we may need it during MapFromObject
                mvcModelItem = await mvcModelItem.MapFromAsync(new TChildEntity()).ConfigureAwait(false);
                mvcModelItem.ParentId = parentId;
                    
                //Init mvc model is it requires async initialization
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (mvcModelItem is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync().ConfigureAwait(false);
                    
                return new TMvcView().RenderDetail(mvcModelItem).ToHtmlResult();
            }

            var entityItem = await GetItemAsync(id).ConfigureAwait(false);
            mvcModelItem = new TChildDetailMvcModel();

            //Init mvc model is it requires async initialization
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (mvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);

            mvcModelItem = await mvcModelItem.MapFromAsync(entityItem).ConfigureAwait(false);

            return new TMvcView().RenderDetail(mvcModelItem).ToHtmlResult();
        }
    }
    public virtual async Task<ActionResult> PutDetailAsync(long id, bool? isInline = null)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id == 0) throw new SupermodelException("CRUDControllerBase.Detail[Post]: id == 0");

            var entityItem = await GetItemAndCacheItAsync(id).ConfigureAwait(false);
            TChildDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix, null).ConfigureAwait(false);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                    
                //Init ex.Model is it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync().ConfigureAwait(false);

                if (isInline == true)
                {
                    var modelState = await SerializableModelState.CreateFromContextAsync().ConfigureAwait(false);
                    HttpContext.Current.TempData[Config.ModelState] = modelState.SerializeToJson();
                    return GoToListScreen(new TChildDetailMvcModel().GetParentEntity(entityItem)!.Id, id);
                }
                else
                {
                    return new TMvcView().RenderDetail((TChildDetailMvcModel)ex.Model).ToHtmlResult();
                }
            }
            return await AfterUpdateAsync(id, entityItem, mvcModelItem).ConfigureAwait(false);
        }
    }
    public virtual async Task<ActionResult> PostDetailAsync(long id, long parentId, bool? isInline = null)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            if (id != 0) throw new SupermodelException("CRUDControllerBase.Detail[Put]: id != 0");

            var entityItem = new TChildEntity();
            TChildDetailMvcModel mvcModelItem;
            try
            {
                var prefix = isInline == true ? Config.InlinePrefix : "";
                (entityItem, mvcModelItem) = await TryUpdateEntityAsync(entityItem, prefix, parentId).ConfigureAwait(false);
            }
            catch (ModelStateInvalidException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                    
                //Init ex.Model is it requires async initialization
                if (ex.Model is IAsyncInit iai && !iai.AsyncInitialized) await iai.InitAsync().ConfigureAwait(false);

                if (isInline == true)
                {
                    var modelState = await SerializableModelState.CreateFromContextAsync().ConfigureAwait(false);
                    HttpContext.Current.TempData[Config.ModelState] = modelState.SerializeToJson();
                    return GoToListScreen(parentId, id);
                }
                else
                {
                    return new TMvcView().RenderDetail((TChildDetailMvcModel)ex.Model).ToHtmlResult();
                }
            }
            entityItem.Add();
            return await AfterCreateAsync(id, parentId, entityItem, mvcModelItem).ConfigureAwait(false);
        }
    }
    public virtual async Task<ActionResult> DeleteDetailAsync(long id)
    {
        TChildEntity? entityItem = null;
        await using (new UnitOfWork<TDataContext>())
        {
            long? parentId = null;
            try
            {
                entityItem = await GetItemAsync(id).ConfigureAwait(false);
                parentId = new TChildDetailMvcModel().GetParentEntity(entityItem)!.Id;
                entityItem.Delete();
            }
            catch (UnableToDeleteException ex)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                HttpContext.Current.TempData.Super().NextPageModalMessage = ex.Message;
            }
            catch (Exception)
            {
                UnitOfWorkContext<TDataContext>.CurrentDataContext.CommitOnDispose = false; //rollback the transaction
                HttpContext.Current.TempData.Super().NextPageModalMessage = "PROBLEM!!!\\n\\nUnable to delete. Most likely reason: references from other entities.";
            }
            if (parentId == null) throw new SupermodelException("Unknown parentId");
            return await AfterDeleteAsync(id, parentId.Value, entityItem!).ConfigureAwait(false);
        }
    }

    public virtual async Task<ActionResult> GetBinaryFileAsync(long id, string pn)
    {
        await using (new UnitOfWork<TDataContext>(ReadOnly.Yes))
        {
            var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(await GetItemAsync(id).ConfigureAwait(false)).ConfigureAwait(false);

            var file = (BinaryFileModelBase?)mvcModelItem.PropertyGet(pn);
            if (file == null || file.IsEmpty) return new StatusCodeResult(HttpStatusCode.NotFound);

            var contentType = MimeTypes.GetMimeType(file.FileName!);
            return new BinaryFileResult(file.BinaryContent!, file.FileName!, contentType, SuggestOpenBinaryFilesInline);
        }
    }
    public virtual async Task<ActionResult> DeleteBinaryFileAsync(long id, string pn)
    {
        await using (new UnitOfWork<TDataContext>())
        {
            var entityItem = await GetItemAsync(id).ConfigureAwait(false);
            var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(entityItem).ConfigureAwait(false);

            //see if pn is a required property
            var propInfo = typeof(TChildDetailMvcModel).GetProperty(pn) ?? throw new SupermodelException("GetProperty(pn) == null");
            if (Attribute.GetCustomAttribute(propInfo, typeof(RequiredAttribute), true) != null)
            {
                HttpContext.Current.TempData.Super().NextPageModalMessage = "Cannot delete required field";
                return StayOnDetailScreen(id);
            }

            var file = (BinaryFileModelBase?)mvcModelItem.PropertyGet(pn);
            if (file == null) throw new SystemException("file == null");

            file.Empty();
            entityItem = await mvcModelItem.MapToAsync(entityItem).ConfigureAwait(false);

            var result = await AfterBinaryDeleteAsync(id, entityItem, mvcModelItem).ConfigureAwait(false);
            await UnitOfWorkContext<TDataContext>.CurrentDataContext.FinalSaveChangesAsync().ConfigureAwait(false);
            return result;
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

    #region Protected Methods & Properties
    protected virtual Task<ActionResult> AfterUpdateAsync(long id, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen(mvcModelItem.GetParentEntity(entityItem)!.Id));
    }
    protected virtual Task<ActionResult> AfterCreateAsync(long id, long parentId, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
    {
        return Task.FromResult(GoToListScreen(mvcModelItem.GetParentEntity(entityItem)!.Id));
    }
    protected virtual Task<ActionResult> AfterDeleteAsync(long id, long parentId, TChildEntity entityItem)
    {
        return Task.FromResult(GoToListScreen(parentId));
    }
    protected virtual Task<ActionResult> AfterBinaryDeleteAsync(long id, TChildEntity entityItem, TChildDetailMvcModel mvcModelItem)
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
        var mvcModelItem = await new TChildDetailMvcModel().MapFromAsync(entityItem).ConfigureAwait(false);
        if (parentId != null) mvcModelItem.ParentId = parentId;
        try
        {
            await TryUpdateModelAsync(mvcModelItem, prefix).ConfigureAwait(false);
            if (!HttpContext.Current.ValidationResultList.IsValid) throw new ModelStateInvalidException(mvcModelItem);

            entityItem = await mvcModelItem.MapToAsync(entityItem).ConfigureAwait(false);
            if (!HttpContext.Current.ValidationResultList.IsValid) throw new ModelStateInvalidException(mvcModelItem);

            //Validation: we only run ValidateAsync() here because attribute-based validation is already picked up by the framework
            var vrl = await mvcModelItem.ValidateAsync(new ValidationContext(mvcModelItem)).ConfigureAwait(false);
            if (vrl.Count != 0)  throw new ValidationResultException(vrl);
            return Tuple.Create(entityItem, mvcModelItem);
        }
        catch (ValidationResultException ex)
        {
            HttpContext.Current.ValidationResultList.AddValidationResultList(ex.ValidationResultList, prefix);
            throw new ModelStateInvalidException(mvcModelItem);
        }        
    }

    protected virtual ActionResult GoToListScreen(long parentId, long? selectedId = null)
    {
        var qsDict = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
        qsDict["parentId"] = parentId.ToString();
        if (selectedId != null) qsDict["selectedId"] = selectedId.ToString();
        return RedirectToAction("List", qsDict);
    }
    protected virtual ActionResult StayOnDetailScreen(long id)
    {
        var qs = HttpContext.Current.HttpListenerContext.Request.QueryString;
        return RedirectToAction("Detail", id, qs);
    }
    protected virtual ActionResult GoToParentDetail(long parentId)
    {
        var qs = HttpContext.Current.HttpListenerContext.Request.QueryString;
        qs.Remove("parentId");
        return RedirectToAction(typeof(TParentController).GetMvcControllerName(), "Detail", parentId, qs);
    }
    #endregion
}