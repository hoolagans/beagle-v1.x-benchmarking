using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Title : Tag
{
    #region Constructors
    public Title(object? attributes = null) : base("title", attributes) { }
    #endregion
}