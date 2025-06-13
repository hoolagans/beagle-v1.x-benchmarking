using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Span : InlineTag
{
    #region Constructors
    public Span(object? attributes = null, bool generateInline = false) : base("span", attributes, generateInline) { }
    #endregion
}