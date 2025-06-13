using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Del : InlineTag
{
    #region Constructors
    public Del(object? attributes = null, bool generateInline = false) : base("del", attributes, generateInline) { }
    #endregion
}