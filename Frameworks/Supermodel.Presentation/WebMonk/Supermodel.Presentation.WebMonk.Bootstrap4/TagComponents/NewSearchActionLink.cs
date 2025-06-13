using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class NewSearchActionLink : HtmlSnippet
    {
        #region Constructors
        public NewSearchActionLink(string linkLabel) : this(new Txt(linkLabel)) { }
            
        public NewSearchActionLink(IGenerateHtml? linkLabel = null)
        {
            linkLabel ??= new Tags
            {
                new Span(new { @class="oi oi-magnifying-glass" }),
                new Txt("&nbsp;New Search"),
            };
                
            var qs = HttpContext.Current.HttpListenerContext.Request.QueryString;
            var controller = HttpContext.Current.PrefixManager.CurrentContextControllerName;
            var attributes = new {id=ScaffoldingSettings.NewSearchButtonId, @class=ScaffoldingSettings.NewSearchButtonCssClass};
                
            Append(Render.ActionLink(linkLabel, controller, "Search", null, qs, attributes));
        }
        #endregion
    }
}