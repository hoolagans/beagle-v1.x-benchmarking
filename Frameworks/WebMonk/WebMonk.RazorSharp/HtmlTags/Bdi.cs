using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Bdi : InlineTag
{
    #region Constructors
    public Bdi(object? attributes = null, bool generateInline = false) : base("bdi", attributes, generateInline) { }
    #endregion
}