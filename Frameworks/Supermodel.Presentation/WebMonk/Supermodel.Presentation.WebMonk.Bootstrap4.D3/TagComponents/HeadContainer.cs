using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models;

public static partial class D3
{ 
    public class HeadContainer : HtmlContainerSnippet
    {
        #region Constructors
        public HeadContainer(object? headAttributes = null)
        {
            var bs4HeadContainer = new Bs4.HeadContainer(headAttributes);
            AppendAndPush(bs4HeadContainer);
            Append(new Link(new { rel="stylesheet", href="/css/britecharts.min.css", type="text/css" }));
            Pop<Bs4.HeadContainer>();
            InnerContent = bs4HeadContainer.InnerContent;
        }
        #endregion
    }
}