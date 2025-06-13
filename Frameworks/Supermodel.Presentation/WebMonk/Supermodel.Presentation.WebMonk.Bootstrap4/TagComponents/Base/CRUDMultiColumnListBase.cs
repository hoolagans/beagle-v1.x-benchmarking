using System;
using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Presentation.WebMonk.Bootstrap4.Extensions;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.ReflectionMapper;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;

public abstract class CRUDMultiColumnListBase : HtmlSnippet
{ 
    #region Constructors
    protected CRUDMultiColumnListBase(IEnumerable<IMvcModelForEntity> items, Type? childControllerType, IGenerateHtml? pageTitle, long? parentId, bool skipAddNew, bool skipDelete, bool viewOnly)
    {
        var controllerName = childControllerType != null ?
            childControllerType.GetMvcControllerName() :
            HttpContext.Current.PrefixManager.CurrentContextControllerName;
        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");

        if (parentId == null || parentId > 0)
        {
            if (pageTitle != null)
            {
                if (parentId == null) Append(new H2(new { @class = Bs4.ScaffoldingSettings.ListTitleCssClass }) { pageTitle });
                else Append(new H2(new { @class = Bs4.ScaffoldingSettings.ChildListTitleCssClass }) { pageTitle });
            }
            AppendAndPush(new Div(new { id = Bs4.ScaffoldingSettings.CRUDListTopDivId, @class = Bs4.ScaffoldingSettings.CRUDListTopDivCssClass }));
                
            if (!skipAddNew)
            {
                var addLabel = new Span(new { @class="oi oi-plus" });
                var id = 0;

                var addQs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
                if (parentId != null) addQs["parentId"] = parentId.ToString();
                    
                Append(new P
                { 
                    Render.ActionLink(addLabel, controllerName, "Detail", id, addQs, new { @class = Bs4.ScaffoldingSettings.CRUDListAddNewCssClass })
                });
            }
                
            AppendAndPush(new Table( new { id = Bs4.ScaffoldingSettings.CRUDListTableId, @class = Bs4.ScaffoldingSettings.CRUDListTableCssClass }));
            AppendAndPush(new Thead());
            AppendAndPush(new Tr());
                
            //Create header using reflection
            var mvcModelType = items.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();
            var mvcModelForHeader = ReflectionHelper.CreateType(mvcModelType);                
            Append(mvcModelForHeader.ToReadOnlyHtmlTableHeader());

            Append(new Th(new { scope="col" }) { new Txt("Actions") });
            Pop<Tr>();
            Pop<Thead>();

            AppendAndPush(new Tbody());
            foreach (var item in items)
            {
                AppendAndPush(new Tr());

                using (HttpContext.Current.PrefixManager.NewPrefix(Config.InlinePrefix, null, controllerName))
                {
                    //Render list columns using reflection
                    Append(item.ToReadOnlyHtmlTableRow());
                }

                var id = item.Id;

                //make sure we keep query string
                var editViewDeleteQs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
                if (parentId != null) editViewDeleteQs["parentId"] = parentId.ToString();

                AppendAndPush(new Td());
                if (!skipDelete) AppendAndPush(new Div(new { @class="btn-group" }));
                if (viewOnly)
                {
                    var viewLinkLabel = new Span(new { @class="oi oi-eye" });
                    Append(Render.ActionLink(viewLinkLabel, controllerName, "Detail", id, editViewDeleteQs, new { @class = Bs4.ScaffoldingSettings.CRUDListEditCssClass }));
                }
                else
                {
                        
                    var editLinkLabel = new Span(new { @class="oi oi-pencil" });
                    Append(Render.ActionLink(editLinkLabel, controllerName, "Detail", id, editViewDeleteQs, new { @class = Bs4.ScaffoldingSettings.CRUDListEditCssClass }));
                }
                if (!skipDelete)
                {
                    var deleteLinkLabel = new Span(new { @class="oi oi-trash" });
                    Append(Render.RESTfulActionLink(deleteLinkLabel, HttpMethod.Delete, controllerName, "Detail", id, editViewDeleteQs, new { @class = Bs4.ScaffoldingSettings.CRUDListDeleteCssClass }, "Are you sure?"));

                    Pop<Div>();
                }
                Pop<Td>();
                Pop<Tr>();
            }
            Pop<Tbody>();
            Pop<Table>();
            Pop<Div>();
        }
    }
    #endregion
}