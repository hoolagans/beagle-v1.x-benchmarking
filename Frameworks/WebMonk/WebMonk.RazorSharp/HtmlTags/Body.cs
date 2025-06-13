using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Body : Tag
{
    #region Constructors
    public Body(object? attributes = null) : base("body", attributes) { }
    #endregion
}