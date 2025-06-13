using System;
using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations.Misc;
using Supermodel.Presentation.WebMonk.Extensions;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class SortByDropdownForm : HtmlSnippet
    {
        #region Constructors
        public SortByDropdownForm(SortByOptions sortByOptions, object? htmlAttributes = null)
        {
            var htmlAttributesDict = AttributesDict.FromAnonymousObject(htmlAttributes);

            AppendAndPush(new Form(new { id=ScaffoldingSettings.SortByDropdownFormId, method = "get" }));
            AppendAndPush(new Fieldset(new { id=ScaffoldingSettings.SortByDropdownFieldsetId }));

            //Get all the query string params, except the dropdown
            var query = HttpContext.Current.HttpListenerContext.Request.QueryString;
            foreach (var queryStringKey in query.Keys)
            {
                var key = queryStringKey!.ToString()!;
                switch (key)
                {
                    case "smSortBy":
                        continue;

                    case "smSkip":
                        Append(new Input(new { id="smSkip", name="smSkip", type="hidden", value="0" }));
                        break;

                    case "_":
                        Append(new Input(new { name="_", type="hidden", value = DateTime.Now.Ticks }));
                        break;

                    default:
                        var id = key.ToHtmlId();
                        var name = key.ToHtmlName();
                        var values = query.GetValues(key);
                        var value = values?.FirstOrDefault() ?? "";
                        Append(new Input(new { id, name, type="hidden", value }));
                        break;
                }
            }

            //Get the dropdown
            var sortBySelectList = new List<SelectListItem> { new( "", "Select Sort Order" ) };
            foreach (var sortByOption in sortByOptions) sortBySelectList.Add(new SelectListItem(sortByOption.Value, $"Sort By: {sortByOption.Key}"));

            //Create an empty dict or a copy of it
            htmlAttributesDict = new AttributesDict(htmlAttributesDict);

            htmlAttributesDict["onchange"] = "this.form.submit();";
            htmlAttributesDict.AddOrAppendCssClass("form-control");
                
            using(HttpContext.Current.PrefixManager.NewPrefix("smSortBy", null))
            {
                var model = HttpContext.Current.HttpListenerContext.Request.QueryString["smSortBy"] ?? "";
                Append(Render.DropdownListForModel(model, sortBySelectList, htmlAttributesDict));
            }

            //end fieldset and form
            Pop<Fieldset>();
            Pop<Form>();
        }  
        #endregion
    }
}