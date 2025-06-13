using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Q : InlineTag
{
    #region Constructors
    public Q(object? attributes = null, bool generateInline = false) : base("q", attributes, generateInline) { }
    #endregion
}