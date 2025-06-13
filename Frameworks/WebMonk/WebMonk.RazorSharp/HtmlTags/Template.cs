using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Template : InlineTag
{
    #region Constructors
    public Template(object? attributes = null, bool generateInline = false) : base("template", attributes, generateInline) { }
    #endregion
}