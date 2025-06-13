using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Abbr : InlineTag
{
    #region Constructors
    public Abbr(object? attributes = null, bool generateInline = false) : base("abbr", attributes, generateInline) { }
    #endregion
}