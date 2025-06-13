using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Datalist : InlineTag
{
    #region Constructors
    public Datalist(object? attributes = null, bool generateInline = false) : base("datalist", attributes, generateInline) { }
    #endregion
}