using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Ruby : InlineTag
{
    #region Constructors
    public Ruby(object? attributes = null, bool generateInline = false) : base("ruby", attributes, generateInline) { }
    #endregion
}