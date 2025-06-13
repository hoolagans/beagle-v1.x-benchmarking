using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Strong : InlineTag
{
    #region Constructors
    public Strong(object? attributes = null, bool generateInline = false) : base("strong", attributes, generateInline) { }
    #endregion
}