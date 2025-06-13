using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Noscript : InlineTag
{
    #region Constructors
    public Noscript(object? attributes = null, bool generateInline = false) : base("Noscript", attributes, generateInline) { }
    #endregion
}