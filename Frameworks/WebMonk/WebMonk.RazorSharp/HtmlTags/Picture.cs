using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Picture : InlineTag
{
    #region Constructors
    public Picture(object? attributes = null, bool generateInline = false) : base("picture", attributes, generateInline) { }
    #endregion
}