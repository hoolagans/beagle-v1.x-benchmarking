using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS instead.")]
public class Basefont : Tag
{
    #region Constructors
    public Basefont(object? attributes = null) : base("basefont", attributes) { }
    #endregion
}