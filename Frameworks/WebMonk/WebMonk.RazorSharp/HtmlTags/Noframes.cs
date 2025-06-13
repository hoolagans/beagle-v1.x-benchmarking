using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Noframes : Tag
{
    #region Constructors
    public Noframes(object? attributes = null) : base("noframes", attributes) { }
    #endregion
}