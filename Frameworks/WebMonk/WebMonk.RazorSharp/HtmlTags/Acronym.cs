using System;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

[Obsolete("Not supported in HTML5, use <abbr> instead.")]
public class Acronym : InlineTag
{
    #region Constructors
    public Acronym(object? attributes = null, bool generateInline = false) : base("acronym", attributes, generateInline) { }
    #endregion
}