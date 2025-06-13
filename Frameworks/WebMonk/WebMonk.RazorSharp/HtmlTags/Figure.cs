using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Figure : Tag
{
    #region Constructors
    public Figure(object? attributes = null) : base("figure", attributes) { }
    #endregion
}