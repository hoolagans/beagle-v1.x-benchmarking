using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Wbr : SelfClosingInlineTag
{
    #region Constructors
    public Wbr(object? attributes = null, bool generateInline = false) : base("wbr", attributes, generateInline) { }
    #endregion
}