using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Pre : TagWithWhiteSpaceText
{
    #region Constructors
    public Pre(object? attributes = null) : base("pre", attributes) { }
    #endregion
}