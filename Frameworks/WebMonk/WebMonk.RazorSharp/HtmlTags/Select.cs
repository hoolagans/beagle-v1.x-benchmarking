using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Select : InlineTag
{
    #region Constructors
    public Select(object? attributes = null, bool generateInline = false) : base("select", attributes, generateInline) { }
    #endregion
}