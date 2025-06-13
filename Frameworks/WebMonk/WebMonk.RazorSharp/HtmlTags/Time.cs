using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Time : InlineTag
{
    #region Constructors
    public Time(object? attributes = null, bool generateInline = false) : base("time", attributes, generateInline) { }
    #endregion
}