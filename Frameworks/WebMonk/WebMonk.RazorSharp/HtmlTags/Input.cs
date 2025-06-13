using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Input : SelfClosingInlineTag
{
    #region Constructors
    public Input(object? attributes = null, bool generateInline = false) : base("input", attributes, generateInline) { }
    #endregion
}