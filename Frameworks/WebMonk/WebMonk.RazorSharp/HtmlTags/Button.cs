using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Button : InlineTag
{
    #region Constructors
    public Button(object? attributes = null, bool generateInline = false) : base("button", attributes, generateInline) { }
    #endregion
}