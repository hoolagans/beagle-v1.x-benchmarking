using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class I : InlineTag
{
    #region Constructors
    public I(object? attributes = null, bool generateInline = false) : base("i", attributes, generateInline) { }
    #endregion
}