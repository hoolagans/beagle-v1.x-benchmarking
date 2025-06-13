using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Option : Tag
{
    #region Constructors
    public Option(object? attributes = null) : base("option", attributes) { }
    #endregion
}