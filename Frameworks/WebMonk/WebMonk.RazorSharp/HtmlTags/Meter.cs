using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Meter : InlineTag
{
    #region Constructors
    public Meter(object? attributes = null, bool generateInline = false) : base("meter", attributes, generateInline) { }
    #endregion
}