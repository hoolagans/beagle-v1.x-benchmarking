using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Label : InlineTag
{
    #region Constructors
    public Label(object? attributes = null, bool generateInline = false) : base("label", attributes, generateInline) { }
    #endregion
}