using System.Web;
using Supermodel.Presentation.WebMonk.Extensions.Gateway;
using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class BodyContainer : HtmlContainerSnippet
    {
        #region Constructors
        public BodyContainer(object? bodyAttributes = null)
        {
            AppendAndPush(new Body(bodyAttributes)); 

            Append(new Script(new { src="/js/jquery-3.6.0.min.js" }));
            Append(new Script(new { src="/js/bootstrap.bundle.min.js" }));
            Append(new Script(new { src="/js/jquery-ui.min.js" }));
            Append(new Script(new { src="/js/bootbox.all.min.js"}));
            Append(new Script(new { src="/js/super.bs4.js" }));

            if (HttpContext.Current.TempData.Super().NextPageStartupScript != null ||
                HttpContext.Current.TempData.Super().NextPageAlertMessage != null ||
                HttpContext.Current.TempData.Super().NextPageModalMessage != null)
            {
                AppendAndPush(new Script());
                Append(new Txt("$(function () {"));

                if (HttpContext.Current.TempData.Super().NextPageStartupScript != null) Append(new Txt(HttpContext.Current.TempData.Super().NextPageStartupScript ?? ""));
                if (HttpContext.Current.TempData.Super().NextPageAlertMessage != null) Append(new Txt($"alert(\"{HttpUtility.HtmlEncode(HttpContext.Current.TempData.Super().NextPageAlertMessage!)}\");"));
                if (HttpContext.Current.TempData.Super().NextPageModalMessage != null) Append(new Txt($"bootbox.alert(\"{HttpUtility.HtmlEncode(HttpContext.Current.TempData.Super().NextPageModalMessage!)}\".replace(/\\n/g, \"<br />\"));"));

                Append(new Txt("});"));

                Pop<Script>();
            }

            Append(InnerContent = new Tags());

            Pop<Body>();
        }
        #endregion
    }
}