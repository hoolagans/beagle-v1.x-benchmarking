using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Article : Tag
{
    #region Constructors
    public Article(object? attributes = null) : base("article", attributes) { }
    #endregion
}