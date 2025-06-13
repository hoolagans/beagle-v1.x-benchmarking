using System;
using System.Collections.Generic;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;

public abstract class CRUDListBase : HtmlSnippet
{ 
    protected CRUDListBase(IEnumerable<IMvcModelForEntity> items, long? parentId, IGenerateHtml? pageTitle, bool skipAddNew, bool skipDelete, bool viewOnly, Type? controllerType)
    {
        var controllerName = controllerType != null ?
            controllerType.GetMvcControllerName() :
            HttpContext.Current.PrefixManager.CurrentContextControllerName;

        if (controllerName == null) throw new SupermodelException("controllerName == null. this should never happen");

        if (parentId == null || parentId > 0)
        {
            if (pageTitle != null)
            {
                if (parentId == null) Append(new H2(new { @class=Bs4.ScaffoldingSettings.ListTitleCssClass}) { pageTitle });
                else Append(new H2(new { @class=Bs4.ScaffoldingSettings.ChildListTitleCssClass}) { pageTitle });
            }
            AppendAndPush(new Div(new { id=Bs4.ScaffoldingSettings.CRUDListTopDivId, @class=Bs4.ScaffoldingSettings.CRUDListTopDivCssClass }));
            if (!skipAddNew)
            {
                //make sure we keep query string
                var qs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
                if (parentId != null) qs["parentId"] = parentId.ToString();
                var id = (long)0;

                //set up html attributes and link label
                var linkLabel = new Span(new { @class="oi oi-plus" });
                var htmlAttributes = new { @class=Bs4.ScaffoldingSettings.CRUDListAddNewCssClass };
                Append(new P {Render.ActionLink(linkLabel, controllerName, "Detail", id, qs, htmlAttributes)} );
            }
            AppendAndPush(new Table(new { id=Bs4.ScaffoldingSettings.CRUDListTableId, @class=Bs4.ScaffoldingSettings.CRUDListTableCssClass }));
            Append(new Thead
            { 
                new Tr
                { 
                    new Th(new { scope = "col" }) { new Txt("Name") }, 
                    new Th(new { scope = "col" }) { new Txt("Actions") }
                } 
            });
            AppendAndPush(new Tbody());

            foreach (var item in items)
            {
                AppendAndPush(new Tr());
                Append(new Td { new Txt(item.Label) });

                //make sure we keep query string
                var qs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
                if (parentId != null) qs["parentId"] = parentId.ToString();
                var id = item.Id;

                AppendAndPush(new Td());
                if (viewOnly)
                {
                    var linkLabel = new Span(new { @class="oi oi-eye" });
                    var htmlAttributes = new { @class=Bs4.ScaffoldingSettings.CRUDListEditCssClass };
                    Append(Render.ActionLink(linkLabel, controllerName, "Detail", id, qs, htmlAttributes));
                }
                else
                {
                    if (!skipDelete) AppendAndPush(new Div(new { @class="btn-group" }));
                    
                    var editLinkLabel = new Span(new { @class="oi oi-pencil" });
                    var editHtmlAttributes = new { @class=Bs4.ScaffoldingSettings.CRUDListEditCssClass };
                    Append(Render.ActionLink(editLinkLabel, controllerName, "Detail", id, qs, editHtmlAttributes));

                    if (!skipDelete)
                    {
                        var deleteLinkLabel = new Span(new { @class="oi oi-trash" });
                        var deleteHtmlAttributes = new { @class=Bs4.ScaffoldingSettings.CRUDListDeleteCssClass };
                        Append(Render.RESTfulActionLink(deleteLinkLabel, HttpMethod.Delete, controllerName, "Detail", id, qs, deleteHtmlAttributes, "Are you sure?"));

                        Pop<Div>();
                    }
                }
                Pop<Td>();
                Pop<Tr>();
            }
            Pop<Tbody>();
            Pop<Table>();
            Pop<Div>();
        }
    }
}