using System.Linq;
using Supermodel.DataAnnotations.Misc;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;
using WebMonk.ValueProviders;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class SortableColumnHeader : HtmlSnippet
    {
        #region Constructors
        public SortableColumnHeader(string headerName, string? orderBy, string? orderByDesc, string? tooltip = null, bool requiredLabel = false, IGenerateHtml? sortedHtml = null, IGenerateHtml? sortedHtmlDesc = null, object? htmlAttributes = null)
        {
            var htmlAttributesDict = AttributesDict.FromAnonymousObject(htmlAttributes);
            if (!string.IsNullOrEmpty(tooltip))
            {
                htmlAttributesDict.Add("data-toggle", "tooltip");
                htmlAttributesDict.Add("title", tooltip);
                headerName += " \u24d8";
            }

            sortedHtml ??= new Txt("▲");
            sortedHtmlDesc ??= new Txt("▼");

            AppendAndPush(new Th());
                
            var query = HttpContext.Current.HttpListenerContext.Request.QueryString;
            var currentSortByValue = (query.GetValues("smSortBy")?.FirstOrDefault() ?? "").Trim();
                
            var controller = HttpContext.Current.PrefixManager.CurrentContextControllerName;
            var action = HttpContext.Current.RouteManager.GetAction();

            var id = HttpContext.Current.ValueProviderManager.GetCachedValueProvidersList().GetValueOrDefault<long?>("id").GetCastValue<long?>();

            var queryStringDict = query.ToQueryStringDictionary();
            queryStringDict["smSkip"] = "0";
            queryStringDict["smSortBy"] = orderBy;

            var queryStringDictDesc = query.ToQueryStringDictionary();
            queryStringDictDesc["smSkip"] = "0";
            queryStringDictDesc["smSortBy"] = orderByDesc;

            var requiredMark = new Tags();
            if (requiredLabel) 
            { 
                requiredMark.Add(new Sup 
                { 
                    new Em(new { @class=$"text-danger font-weight-bold {ScaffoldingSettings.RequiredAsteriskCssClass}" })
                    {
                        new Txt("*"),
                    }
                }); 
            }

            if (currentSortByValue == orderBy)
            {
                if (!string.IsNullOrEmpty(orderByDesc)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDictDesc, htmlAttributesDict));
                    Append(requiredMark);
                    Append(sortedHtml);
                }
                else if (!string.IsNullOrEmpty(orderBy)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDict, htmlAttributesDict));
                    Append(requiredMark);
                    Append(sortedHtml);
                }
                else 
                {
                    Append(new Txt(headerName));
                    Append(requiredMark);
                }
            }
            else if (currentSortByValue == orderByDesc)
            {
                if (!string.IsNullOrEmpty(orderBy)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDict, htmlAttributesDict));
                    Append(requiredMark);
                    Append(sortedHtmlDesc);
                }
                else if (!string.IsNullOrEmpty(orderByDesc)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDictDesc, htmlAttributesDict));
                    Append(requiredMark);
                    Append(sortedHtmlDesc);
                }
                else 
                {
                    Append(new Txt(headerName));
                    Append(requiredMark);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(orderBy)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDict, htmlAttributesDict));
                    Append(requiredMark);
                }
                else if (!string.IsNullOrEmpty(orderByDesc)) 
                {
                    Append(Render.ActionLink(headerName, controller, action!, id, queryStringDictDesc, htmlAttributesDict));
                    Append(requiredMark);
                }
                else
                {
                    Append(new Span(htmlAttributesDict) { new Txt(headerName) });
                    Append(requiredMark);
                }
            }
            Pop<Th>();
        }
        #endregion
    }
}