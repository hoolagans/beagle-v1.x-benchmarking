using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS instead.")]
public class Font : Tag
{
    #region Constructors
    public Font(object? attributes = null) : base("font", attributes) { }
    #endregion
}