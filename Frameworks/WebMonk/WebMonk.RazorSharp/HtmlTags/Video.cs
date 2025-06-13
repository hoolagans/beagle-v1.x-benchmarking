using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Video : InlineTag
{
    #region Constructors
    public Video(object? attributes = null, bool generateInline = false) : base("video", attributes, generateInline) { }
    #endregion
}