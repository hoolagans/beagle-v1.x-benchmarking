using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Img : SelfClosingInlineTag
{
    #region Constructors
    public Img(object? attributes = null, bool generateInline = false) : base("img", attributes, generateInline) { }
    #endregion
}