using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Source : SelfClosingTag
{
    #region Constructors
    public Source(object? attributes = null) : base("source", attributes) { }
    #endregion
}