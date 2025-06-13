using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Li : Tag
{
    #region Constructors
    public Li(object? attributes = null) : base("li", attributes) { }
    #endregion
}