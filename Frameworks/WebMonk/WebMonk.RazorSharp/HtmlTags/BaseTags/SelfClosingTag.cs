using System;
using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class SelfClosingTag : Tag
{
    #region Constructors
    public SelfClosingTag(string? tagType, object? attributes = null) : base(tagType, attributes) { }
    #endregion

    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        sb.AppendLine($"<{TagType}{GenerateMyAttributesString()} />");
        return sb;
    }
    // ReSharper disable once UnusedParameter.Global
    public new void Add(IGenerateHtml item)
    {
        throw new InvalidOperationException("Self-closing tags cannot contain other tags");
    }
    #endregion
}