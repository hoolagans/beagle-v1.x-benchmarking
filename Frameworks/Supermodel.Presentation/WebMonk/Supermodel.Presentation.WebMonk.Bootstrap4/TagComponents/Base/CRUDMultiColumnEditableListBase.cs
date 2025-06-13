using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations;
using Supermodel.DataAnnotations.Async;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.UnitOfWork;
using Supermodel.Presentation.WebMonk.Bootstrap4.Extensions;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.Misc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;

public abstract class CRUDMultiColumnEditableListBase : HtmlSnippet
{
    #region Constructors
    protected CRUDMultiColumnEditableListBase(IEnumerable<IMvcModelForEntity> items, Type dataContextType, Type? detailControllerType, IGenerateHtml? pageTitle, long? parentId, bool skipAddNew, bool skipDelete)
    {
        var controllerName = detailControllerType != null ?
            detailControllerType.GetMvcControllerName() :
            HttpContext.Current.PrefixManager.CurrentContextControllerName;
        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");

        if (parentId == null || parentId > 0)
        { 
            if (pageTitle != null) Append(new H2(new { @class=Bs4.ScaffoldingSettings.ListTitleCssClass }) { pageTitle });  
            
            //var query = Html.ViewContext.HttpContext.Request.Query;
            var qs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
            qs.Remove("selectedId");
            if (parentId != null) qs["parentId"] = parentId.ToString();

            var formAction = Render.Helper.UrlForMvcAction(controllerName, "Detail", null, qs);
            AppendAndPush(new Form(new { id=Bs4.ScaffoldingSettings.EditFormId, action=formAction, method="post", enctype="multipart/form-data" }));
            AppendAndPush(new Fieldset(new { id=Bs4.ScaffoldingSettings.EditFormFieldsetId }));
            AppendAndPush(new Div(new { id=Bs4.ScaffoldingSettings.CRUDListTopDivId, @class=Bs4.ScaffoldingSettings.CRUDListTopDivCssClass }));
                
            //var selectedId = (long?)Html.ViewBag.SelectedId ?? ParseNullableLong(Html.ViewContext.HttpContext.Request.Query["selectedId"]);
            var selectedId = ParseNullableLong(HttpContext.Current.HttpListenerContext.Request.QueryString.GetValues("selectedId")?.SingleOrDefault());
                
            //This could be a potential scalability issue, but I can't figure out how to solve it for now
            var newItem = AsyncHelper.RunSync(() => GetNewItemAsync(items.GetType(), dataContextType));
                
            // ReSharper disable once PossibleMultipleEnumeration
            var selectedItem = selectedId == 0 ? newItem : items.SingleOrDefault(x => x.Id == selectedId);
            var anySelected = selectedItem != null;

            if (!skipAddNew)
            {
                //make sure we keep query string
                //var addQs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToStringDictionary();
                //id = "0";

                Append(new P 
                { 
                    new Button(new { type="button", disabled= anySelected ? "disabled" : null, data_open_new_for_edit="data-open-new-for-edit", @class=Bs4.ScaffoldingSettings.CRUDListAddNewCssClass })
                    {
                        new Span(new { @class="oi oi-plus" }),
                    }
                });
            }
                
            AppendAndPush(new Table(new { id=Bs4.ScaffoldingSettings.CRUDListTableId, @class=Bs4.ScaffoldingSettings.CRUDListTableCssClass }));
            AppendAndPush(new Thead());
            AppendAndPush(new Tr());
            Append(newItem.ToEditableHtmlTableHeader());
            Append(new Th(new { scope="col" } ){ new Txt("Actions") });
            Pop<Tr>();
            Pop<Thead>();
                
            AppendAndPush(new Tbody());

            //do html for selected item so that we could clear the ModelState, so that other forms do not pick up the data from ModelState
            IGenerateHtml? selectedItemTrHtml = null; 
            if (selectedItem != null)
            {
                using(HttpContext.Current.PrefixManager.NewPrefix(Config.InlinePrefix, null, controllerName))
                {
                    selectedItemTrHtml = selectedId == 0 ? 
                        MakeNewItemEditableTr(selectedItem, parentId, true) : 
                        MakeEditableTr(selectedItem, parentId, true);
                }
            }
            HttpContext.Current.ValidationResultList.Clear();
                
            //new item 
            var selected = newItem.Id == selectedId;
            if (selected)
            {
                if (selectedItemTrHtml == null) throw new SupermodelException("selectedItemTrHtml == null: this should never happen");
                Append(selectedItemTrHtml);
            }
            else
            {
                using(HttpContext.Current.PrefixManager.NewPrefix(Config.InlinePrefix, null, controllerName))
                {
                    Append(MakeNewItemEditableTr(newItem, parentId, false));
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var item in items)
            {
                //are we dealing with a selected item?
                selected = item.Id == selectedId;
                    
                //make sure we keep query string
                var editViewDeleteQs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
                    
                using (HttpContext.Current.PrefixManager.NewPrefix(Config.InlinePrefix, null, controllerName))
                {
                    Append(MakeReadOnlyTr(item, parentId, selected, anySelected, skipDelete, controllerName, editViewDeleteQs));

                    if (selected) 
                    {
                        if (selectedItemTrHtml == null) throw new SupermodelException("selectedItemTrHtml == null: this should never happen");
                        Append(selectedItemTrHtml);
                    }
                    else 
                    {
                        Append(MakeEditableTr(item, parentId, false));
                    }
                }
            }
            Pop<Tbody>();
            Pop<Table>();
            Pop<Div>();
            Pop<Fieldset>();
            Pop<Form>();
        }
    }
    #endregion

    #region Protected Helpers
    protected IGenerateHtml MakeReadOnlyTr(IViewModelForEntity item, long? parentId, bool selected, bool anySelected, bool skipDelete, string controllerName, QueryStringDict editViewDeleteQs)
    {
        var result = new HtmlStack();
        //var result = new HtmlStack(new Tr(new { id=item.Id, @class = selected ? "d-none" : null}));
        result.AppendAndPush(new Tr(new { id=item.Id, @class = selected ? "d-none" : null}));

        result.Append(item.ToEditableHtmlTableRow(parentId, false, false));
        result.AppendAndPush(new Td());
        result.AppendAndPush(new Div(new { @class=anySelected ? "btn-group d-none" : "btn-group", data_read_only_tr_buttons="data-read-only-tr-buttons" }));
        result.AppendAndPush(new Button(new { type="button", data_open_for_edit="data-open-for-edit", @class=Bs4.ScaffoldingSettings.CRUDListEditCssClass} ));
        result.Append(new Span(new { @class="oi oi-pencil" }));
        result.Pop<Button>();
        if (!skipDelete) result.Append(Render.RESTfulActionLink(new Span(new { @class="oi oi-trash"}), HttpMethod.Delete, controllerName, "Detail", item.Id, editViewDeleteQs, new { @class = Bs4.ScaffoldingSettings.CRUDListDeleteCssClass, data_delete = "data-delete" }, "Are you sure?"));
        result.Pop<Div>();
        result.Pop<Td>();

        result.Pop<Tr>();
        return result;
    }
    protected IGenerateHtml MakeEditableTr(IViewModelForEntity item, long? parentId, bool selected)
    {
        return new Tr(new { id=-item.Id, @class = selected ? "table-primary" : "table-primary d-none" })
        {
            item.ToEditableHtmlTableRow(parentId, true, selected),
            new Td
            {
                new Div(new { @class="btn-group" })
                {
                    new Button(new { type="submit", @class=Bs4.ScaffoldingSettings.CRUDListSaveCssClass, data_save_edit="data-save-edit" })
                    {
                        new Span(new { @class="oi oi-circle-check" })

                    },
                    new Button(new { type="button", data_cancel_edit="data-cancel-edit", @class=Bs4.ScaffoldingSettings.CRUDListCancelCssClass })
                    {
                        new Span(new { @class="oi oi-action-undo" })

                    },
                }
            }
        };
    }
    protected IGenerateHtml MakeNewItemEditableTr(IViewModelForEntity newItem, long? parentId, bool selected)
    {
        return new Tr(new { id=newItem.Id, @class = selected ? "table-primary" : "table-primary d-none"})
        {
            newItem.ToEditableHtmlTableRow(parentId, true, selected),
            new Td
            {
                new Div(new { @class="btn-group" })
                {
                    new Button(new { type="submit", @class=Bs4.ScaffoldingSettings.CRUDListSaveCssClass, data_save_new_edit="data-save-new-edit"})
                    {
                        new Span(new { @class="oi oi-circle-check" })
                    },
                    new Button(new { type="button", data_cancel_new_edit="data-cancel-new-edit", @class=Bs4.ScaffoldingSettings.CRUDListCancelCssClass })
                    {
                        new Span(new { @class="oi oi-action-undo" })
                    }
                }
            }
        };
    }
    protected virtual async Task<IViewModelForEntity> GetNewItemAsync(Type iEnumerableType, Type dataContextType)
    {
        await using((IAsyncDisposable)ReflectionHelper.CreateGenericType(typeof(UnitOfWork<>), dataContextType, ReadOnly.Yes))
        {
            if (!iEnumerableType.IsGenericType) throw new SupermodelException("!iEnumerableType.IsGenericType");

            var mvcModelItemType = iEnumerableType.GenericTypeArguments.First();
            var newMvcModelItem = (IViewModelForEntity)ReflectionHelper.CreateType(mvcModelItemType);

            //Init mvc model if it requires async initialization
            if (newMvcModelItem is IAsyncInit iAsyncInit && !iAsyncInit.AsyncInitialized) await iAsyncInit.InitAsync().ConfigureAwait(false);
            
            var newEntityItem = newMvcModelItem.CreateEntity();
            newMvcModelItem = await newMvcModelItem.MapFromAsync(newEntityItem).ConfigureAwait(false);
            
            return newMvcModelItem;
        }
    }
    protected long? ParseNullableLong(string? str)
    {
        if (str == null) return null;
        if (long.TryParse(str, out var result)) return result;
        return null;
    }
    #endregion
}