using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Code : InlineTag
{
    #region Constructors
    public Code(object? attributes = null, bool generateInline = false) : base("code", attributes, generateInline) { }
    #endregion
}