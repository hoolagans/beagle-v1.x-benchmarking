using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Canvas : InlineTag
{
    #region Constructors
    public Canvas(object? attributes = null, bool generateInline = false) : base("canvas", attributes, generateInline) { }
    #endregion
}