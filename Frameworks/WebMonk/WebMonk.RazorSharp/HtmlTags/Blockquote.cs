using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Blockquote : Tag
{
    #region Constructors
    public Blockquote(object? attributes = null) : base("Blockquote", attributes) { }
    #endregion
}