using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS, <s>, or <del> instead.")]
public class Strike : Tag
{
    #region Constructors
    public Strike(object? attributes = null) : base("strike", attributes) { }
    #endregion
}