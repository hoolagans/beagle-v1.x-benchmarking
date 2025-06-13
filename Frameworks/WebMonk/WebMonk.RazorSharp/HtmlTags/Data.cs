using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Data : InlineTag
{
    #region Constructors
    public Data(object? attributes = null, bool generateInline = false) : base("data", attributes, generateInline) { }
    #endregion
}