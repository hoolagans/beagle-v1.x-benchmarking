using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class B : InlineTag
{
    #region Constructors
    public B(object? attributes = null, bool generateInline = false) : base("b", attributes, generateInline) { }
    #endregion
}