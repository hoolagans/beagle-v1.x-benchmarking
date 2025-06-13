using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class P : Tag
{
    #region Constructors
    public P(object? attributes = null) : base("p", attributes) { }
    #endregion
}