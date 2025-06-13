using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Small : InlineTag
{
    #region Constructors
    public Small(object? attributes = null, bool generateInline = false) : base("small", attributes, generateInline) { }
    #endregion
}