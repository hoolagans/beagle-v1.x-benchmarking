using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Base : SelfClosingTag
{
    #region Constructors
    public Base(object? attributes = null) : base("base", attributes) { }
    #endregion
}