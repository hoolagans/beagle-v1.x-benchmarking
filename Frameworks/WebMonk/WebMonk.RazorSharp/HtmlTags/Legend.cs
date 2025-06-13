using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Legend : Tag
{
    #region Constructors
    public Legend(object? attributes = null) : base("legend", attributes) { }
    #endregion
}