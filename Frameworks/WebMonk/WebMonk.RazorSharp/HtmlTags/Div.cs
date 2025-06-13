using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Div : Tag
{
    #region Constructors
    public Div(object? attributes = null) : base("div", attributes) { }
    #endregion
}