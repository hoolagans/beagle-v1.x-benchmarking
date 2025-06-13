using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Frameset : Tag
{
    #region Constructors
    public Frameset(object? attributes = null) : base("frameset", attributes) { }
    #endregion
}