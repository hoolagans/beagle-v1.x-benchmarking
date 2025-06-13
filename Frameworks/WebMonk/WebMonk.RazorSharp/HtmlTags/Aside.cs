using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Aside : Tag
{
    #region Constructors
    public Aside(object? attributes = null) : base("aside", attributes) { }
    #endregion
}