using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Samp : InlineTag
{
    #region Constructors
    public Samp(object? attributes = null, bool generateInline = false) : base("samp", attributes, generateInline) { }
    #endregion
}