using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use <object> instead.")]
public class Applet : Tag
{
    #region Constructors
    public Applet(object? attributes = null) : base("applet", attributes) { }
    #endregion
}