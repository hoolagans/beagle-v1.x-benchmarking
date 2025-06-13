using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Form : Tag
{
    #region Constructors
    public Form(object? attributes = null) : base("form", attributes) { }
    #endregion
}