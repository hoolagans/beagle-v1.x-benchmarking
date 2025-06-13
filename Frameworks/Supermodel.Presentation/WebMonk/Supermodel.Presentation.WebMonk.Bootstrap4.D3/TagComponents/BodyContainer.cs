using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.D3.Models;

public static partial class D3
{ 
    public class BodyContainer : HtmlContainerSnippet
    {
        #region Constructors
        public BodyContainer(object? bodyAttributes = null)
        {
            var bs4BodyContainer = new Bs4.BodyContainer(bodyAttributes);
            AppendAndPush(bs4BodyContainer);
            Append(new Script(new { src="/js/d3.v5.min.js" }));
            Append(new Script(new { src="/js/britecharts.min.js" }));
            Pop<Bs4.BodyContainer>();
            InnerContent = bs4BodyContainer.InnerContent;
        }
        #endregion
    }
}