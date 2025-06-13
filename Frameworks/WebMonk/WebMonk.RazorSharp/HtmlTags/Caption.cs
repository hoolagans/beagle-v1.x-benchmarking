using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Caption : Tag
{
    #region Constructors
    public Caption(object? attributes = null) : base("caption", attributes) { }
    #endregion
}