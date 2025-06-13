using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Audio : InlineTag
{
    #region Constructors
    public Audio(object? attributes = null, bool generateInline = false) : base("audio", attributes, generateInline) { }
    #endregion
}