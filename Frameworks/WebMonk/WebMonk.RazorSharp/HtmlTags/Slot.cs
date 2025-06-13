using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Slot : InlineTag
{
    #region Constructors
    public Slot(object? attributes = null, bool generateInline = false) : base("slot", attributes, generateInline) { }
    #endregion
}