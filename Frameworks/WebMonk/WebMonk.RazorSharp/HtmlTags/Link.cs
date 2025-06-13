using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Link : SelfClosingTag
{
    #region Constructors
    public Link(object? attributes = null) : base("link", attributes) { }
    #endregion
}