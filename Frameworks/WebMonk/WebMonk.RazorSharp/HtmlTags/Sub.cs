using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Sub : InlineTag
{
    #region Constructors
    public Sub(object? attributes = null, bool generateInline = false) : base("sub", attributes, generateInline) { }
    #endregion
}