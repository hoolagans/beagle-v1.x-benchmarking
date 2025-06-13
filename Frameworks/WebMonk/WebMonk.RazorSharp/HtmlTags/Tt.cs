using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use CSS instead.")]
public class Tt : InlineTag
{
    #region Constructors
    public Tt(object? attributes = null, bool generateInline = false) : base("tt", attributes, generateInline) { }
    #endregion
}