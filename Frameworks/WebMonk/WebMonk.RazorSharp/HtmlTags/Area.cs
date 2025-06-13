using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Area : SelfClosingTag
{
    #region Constructors
    public Area(object? attributes = null) : base("area", attributes) { }
    #endregion
}