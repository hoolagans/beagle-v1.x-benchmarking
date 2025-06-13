using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Br : SelfClosingTag
{
    #region Constructors
    public Br(object? attributes = null) : base("br", attributes) { }
    #endregion
}