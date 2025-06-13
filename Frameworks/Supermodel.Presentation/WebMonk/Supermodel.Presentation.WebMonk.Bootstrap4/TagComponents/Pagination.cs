using System;
using System.Globalization;
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
    public class Pagination : HtmlSnippet
    {
        #region Constructors
        public Pagination(int totalCount, int visiblePages = 11, int? skip = null, int? take = null)
        {
            if (visiblePages < 3 || visiblePages % 2 == 0) throw new ArgumentException("Must be >=3 and odd", nameof(visiblePages));
                
            var query =  HttpContext.Current.HttpListenerContext.Request.QueryString;
            skip ??= query.GetSkipValue() ?? 0;
            take ??= query.GetTakeValue();
                
            if (take != null && take < totalCount) //skip is never null
            {
                AppendAndPush(new Nav());
                AppendAndPush(new Ul(new { @class=$"pagination {ScaffoldingSettings.PaginationCssClass}" }));
                    
                var currentPage = skip.Value / take.Value + 1;

                var firstPage = currentPage - visiblePages / 2;
                if (firstPage < 1) firstPage = 1;

                var lastPage = firstPage + visiblePages - 1;
                if (lastPage > totalCount / take)
                {
                    firstPage -= lastPage - totalCount / take.Value;
                    if (firstPage < 1) firstPage = 1;
                    lastPage = (int)Math.Ceiling((double)totalCount / take.Value);
                }

                //Prev page
                if (currentPage > 1)
                {
                    Append(new Li(new { @class="page-item" })
                    {
                        GetPageActionLink("«", currentPage - 1, take.Value)
                    });
                }
                else 
                {
                    Append(new Li(new { @class="page-item disabled" })
                    {
                        new A(new { href="#", @class="page-link", tabindex="-1" }) { new Txt("«") }
                    });
                }

                //Neighboring pages
                for (var page = firstPage; page <= lastPage; page++)
                {
                    var link = GetPageActionLink(page.ToString(CultureInfo.InvariantCulture), page, take.Value);
                    if (page == currentPage) Append(new Li(new { @class="page-item active" }) { link });
                    else Append(new Li(new { @class="page-item" }) { link });
                }

                //Next page
                if (currentPage < lastPage) 
                {
                    Append(new Li(new { @class="page-item" }) 
                    { 
                        GetPageActionLink("»", currentPage + 1, take.Value) 
                    });
                }
                else 
                {
                    Append(new Li(new { @class="page-item disabled" }) 
                    { 
                        new A(new { href="#", @class="page-link", tabindex="-1" }) { new Txt("»") }
                    }); 
                }

                Pop<Ul>();
                Pop<Nav>();
            }
        }
        private IGenerateHtml GetPageActionLink(string linkText, int pageNum, int pageSize)
        {
            var controller = HttpContext.Current.PrefixManager.CurrentContextControllerName;
            var action = HttpContext.Current.RouteManager.GetAction();
            var qs = HttpContext.Current.HttpListenerContext.Request.QueryString.ToQueryStringDictionary();
            qs["smSkip"] = ((pageNum - 1) * pageSize).ToString();
            return Render.ActionLink(linkText, controller, action!, null, qs, new { Class = "page-link"});
        }            
        #endregion
    }
}