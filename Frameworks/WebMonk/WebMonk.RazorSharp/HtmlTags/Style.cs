using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Style : Tag
{
    #region Constructors
    public Style(object? attributes = null) : base("style", attributes) { }
    #endregion
}