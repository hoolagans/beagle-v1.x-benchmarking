using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5.")]
public class Nobr : Tag
{
    #region Constructors
    public Nobr(object? attributes = null) : base("nobr", attributes) { }
    #endregion
}