using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class S : InlineTag
{
    #region Constructors
    public S(object? attributes = null, bool generateInline = false) : base("s", attributes, generateInline) { }
    #endregion
}