using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Track : SelfClosingTag
{
    #region Constructors
    public Track(object? attributes = null) : base("track", attributes) { }
    #endregion
}