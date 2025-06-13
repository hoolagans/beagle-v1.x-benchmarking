using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS instead.")]
public class Center : Tag
{
    #region Constructors
    public Center(object? attributes = null) : base("center", attributes) { }
    #endregion
}