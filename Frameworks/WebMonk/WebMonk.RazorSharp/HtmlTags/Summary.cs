using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Summary : Tag
{
    #region Constructors
    public Summary(object? attributes = null) : base("summary", attributes) { }
    #endregion
}