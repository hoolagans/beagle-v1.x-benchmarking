using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Mark : InlineTag
{
    #region Constructors
    public Mark(object? attributes = null, bool generateInline = false) : base("mark", attributes, generateInline) { }
    #endregion
}