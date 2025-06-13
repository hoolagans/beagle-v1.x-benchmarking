using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Svg : InlineTag
{
    #region Constructors
    public Svg(object? attributes = null, bool generateInline = false) : base("svg", attributes, generateInline) { }
    #endregion
}