using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Cite : InlineTag
{
    #region Constructors
    public Cite(object? attributes = null, bool generateInline = false) : base("cite", attributes, generateInline) { }
    #endregion
}