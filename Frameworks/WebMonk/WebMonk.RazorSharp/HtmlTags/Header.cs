using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Header : Tag
{
    #region Constructors
    public Header(object? attributes = null) : base("header", attributes) { }
    #endregion
}