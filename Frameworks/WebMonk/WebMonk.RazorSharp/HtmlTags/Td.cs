using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Td : Tag
{
    #region Constructors
    public Td(object? attributes = null) : base("td", attributes) { }
    #endregion
}