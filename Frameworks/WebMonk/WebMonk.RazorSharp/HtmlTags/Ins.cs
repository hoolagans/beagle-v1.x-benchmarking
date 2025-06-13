using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Ins : InlineTag
{
    #region Constructors
    public Ins(object? attributes = null, bool generateInline = false) : base("ins", attributes, generateInline) { }
    #endregion
}