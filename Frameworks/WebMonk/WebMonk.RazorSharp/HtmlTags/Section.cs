using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Section : Tag
{
    #region Constructors
    public Section(object? attributes = null) : base("section", attributes) { }
    #endregion
}