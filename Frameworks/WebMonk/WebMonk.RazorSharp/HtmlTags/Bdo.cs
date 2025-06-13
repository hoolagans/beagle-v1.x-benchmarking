using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Bdo : InlineTag
{
    #region Constructors
    public Bdo(object? attributes = null, bool generateInline = false) : base("bdo", attributes, generateInline) { }
    #endregion
}