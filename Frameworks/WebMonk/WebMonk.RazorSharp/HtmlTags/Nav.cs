using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Nav : Tag
{
    #region Constructors
    public Nav(object? attributes = null) : base("nav", attributes) { }
    #endregion
}