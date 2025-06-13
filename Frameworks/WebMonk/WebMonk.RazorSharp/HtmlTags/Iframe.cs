using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Iframe : InlineTag
{
    #region Constructors
    public Iframe(object? attributes = null, bool generateInline = false) : base("iframe", attributes, generateInline) { }
    #endregion
}