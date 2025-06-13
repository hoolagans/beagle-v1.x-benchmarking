using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Dfn : InlineTag
{
    #region Constructors
    public Dfn(object? attributes = null, bool generateInline = false) : base("dfn", attributes, generateInline) { }
    #endregion
}