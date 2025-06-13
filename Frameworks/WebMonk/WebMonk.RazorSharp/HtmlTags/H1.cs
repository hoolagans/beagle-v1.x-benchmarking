using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class H1 : Tag
{
    #region Constructors
    public H1(object? attributes = null) : base("h1", attributes) { }
    #endregion
}