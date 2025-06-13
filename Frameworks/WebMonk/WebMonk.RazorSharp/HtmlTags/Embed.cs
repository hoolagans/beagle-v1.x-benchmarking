using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Embed : SelfClosingInlineTag
{
    #region Constructors
    public Embed(object? attributes = null, bool generateInline = false) : base("embed", attributes, generateInline) { }
    #endregion
}