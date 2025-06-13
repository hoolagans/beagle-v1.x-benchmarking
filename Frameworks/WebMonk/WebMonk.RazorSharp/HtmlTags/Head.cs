using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Head : Tag
{
    #region Constructors
    public Head(object? attributes = null) : base("head", attributes) { }
    #endregion
}