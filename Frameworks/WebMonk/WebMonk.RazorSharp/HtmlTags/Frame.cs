using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Frame : Tag
{
    #region Constructors
    public Frame(object? attributes = null) : base("frame", attributes) { }
    #endregion
}