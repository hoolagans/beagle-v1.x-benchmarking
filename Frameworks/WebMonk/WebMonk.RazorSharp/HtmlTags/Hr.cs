using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Hr : SelfClosingTag
{
    #region Constructors
    public Hr(object? attributes = null) : base("hr", attributes) { }
    #endregion
}