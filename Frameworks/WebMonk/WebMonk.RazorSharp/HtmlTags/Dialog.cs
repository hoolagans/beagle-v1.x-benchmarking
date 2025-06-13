using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Dialog : Tag
{
    #region Constructors
    public Dialog(object? attributes = null) : base("dialog", attributes) { }
    #endregion
}