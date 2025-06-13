using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Sup : InlineTag
{
    #region Constructors
    public Sup(object? attributes = null, bool generateInline = false) : base("sup", attributes, generateInline) { }
    #endregion
}