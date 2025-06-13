using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Em : InlineTag
{
    #region Constructors
    public Em(object? attributes = null, bool generateInline = false) : base("em", attributes, generateInline) { }
    #endregion
}