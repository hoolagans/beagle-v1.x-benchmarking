using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class A : InlineTag
{
    #region Constructors
    public A(object? attributes = null, bool generateInline = false) : base("a", attributes, generateInline) { }
    #endregion
}