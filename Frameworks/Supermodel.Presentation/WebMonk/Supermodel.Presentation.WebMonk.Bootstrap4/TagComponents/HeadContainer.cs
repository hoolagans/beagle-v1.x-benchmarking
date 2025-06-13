using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class HeadContainer : HtmlContainerSnippet
    {
        #region Constructors
        public HeadContainer(object? headAttributes = null)
        {
            AppendAndPush(new Head(headAttributes));
                
            Append(new Meta(new { charset="utf-8"}));
            Append(new Meta(new { name="viewport", content="width=device-width, initial-scale=1, shrink-to-fit=no"}));

            Append(new Link(new { rel="stylesheet", href="/css/bootstrap.min.css" }));
            Append(new Link(new { rel="stylesheet", href="/open_iconic/font/css/open-iconic-bootstrap.min.css" }));
            Append(new Link(new { rel="stylesheet", href="/css/jquery-ui.min.css" }));
            Append(new Link(new { rel="stylesheet", href="/css/super.bs4.css" }));

            Append(InnerContent = new Tags());

            Pop<Head>();
        }
        #endregion
    }
}