using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class TagWithWhiteSpaceText : Tag
{
    #region Constructors
    public TagWithWhiteSpaceText(string? tagType, object? attributes = null) : base(tagType, attributes) { }
    #endregion

    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();

        sb.Append($"<{TagType}{GenerateMyAttributesString()}>");
        foreach (var tag in this) 
        {
            if (tag is Txt txtTag) txtTag.ToHtmlNoNewLineAtTheEnd(sb);
            else sb = tag.ToHtml(sb);
        }
        sb.AppendLine($"</{TagType}>");

        return sb;
    }
    #endregion
}