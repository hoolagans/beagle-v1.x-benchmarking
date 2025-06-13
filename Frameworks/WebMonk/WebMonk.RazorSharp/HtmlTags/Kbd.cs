using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Kbd : InlineTag
{
    #region Constructors
    public Kbd(object? attributes = null, bool generateInline = false) : base("kbd", attributes, generateInline) { }
    #endregion
}