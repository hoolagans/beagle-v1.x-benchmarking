using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Textarea : TagWithWhiteSpaceText
{
    #region Constructors
    public Textarea(object? attributes = null) : base("textarea", attributes) { }
    #endregion
}