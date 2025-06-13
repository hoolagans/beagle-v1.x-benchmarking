using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Figcaption : Tag
{
    #region Constructors
    public Figcaption(object? attributes = null) : base("figcaption", attributes) { }
    #endregion
}