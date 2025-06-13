using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Param : SelfClosingTag
{
    #region Constructors
    public Param(object? attributes = null) : base("param", attributes) { }
    #endregion
}