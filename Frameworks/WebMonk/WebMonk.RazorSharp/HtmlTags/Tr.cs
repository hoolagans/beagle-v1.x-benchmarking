using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Tr : Tag
{
    #region Constructors
    public Tr(object? attributes = null) : base("tr", attributes) { }
    #endregion
}