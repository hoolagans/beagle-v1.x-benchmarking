using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Col : SelfClosingTag
{
    #region Constructors
    public Col(object? attributes = null) : base("col", attributes) { }
    #endregion
}