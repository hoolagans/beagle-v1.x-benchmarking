using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Progress : InlineTag
{
    #region Constructors
    public Progress(object? attributes = null, bool generateInline = false) : base("progress", attributes, generateInline) { }
    #endregion
}