using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Tbody : Tag
{
    #region Constructors
    public Tbody(object? attributes = null) : base("tbody", attributes) { }
    #endregion
}