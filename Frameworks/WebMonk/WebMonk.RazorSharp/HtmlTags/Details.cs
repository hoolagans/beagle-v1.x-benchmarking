using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Details : Tag
{
    #region Constructors
    public Details(object? attributes = null) : base("details", attributes) { }
    #endregion
}