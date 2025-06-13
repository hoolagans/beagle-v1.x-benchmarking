using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Object : InlineTag
{
    #region Constructors
    public Object(object? attributes = null, bool generateInline = false) : base("object", attributes, generateInline) { }
    #endregion
}