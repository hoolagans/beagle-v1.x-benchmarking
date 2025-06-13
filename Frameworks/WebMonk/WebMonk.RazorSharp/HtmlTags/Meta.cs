using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Meta : SelfClosingTag
{
    #region Constructors
    public Meta(object? attributes = null) : base("meta", attributes) { }
    #endregion
}