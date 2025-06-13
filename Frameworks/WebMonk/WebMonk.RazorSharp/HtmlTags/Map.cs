using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Map : InlineTag
{
    #region Constructors
    public Map(object? attributes = null, bool generateInline = false) : base("map", attributes, generateInline) { }
    #endregion
}