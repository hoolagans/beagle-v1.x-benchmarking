using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class InlineTag : Tag
{
    #region Constructors
    public InlineTag(string? tagType, object? attributes, bool generateInline) : base(tagType, attributes) 
    { 
        GenerateInline = generateInline;
    }
    #endregion

    #region Overrides
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        if (GenerateInline)
        {
            sb ??= new StringBuilderWithIndents();

            if (ContainsInnerHtml())
            {
                sb.TrimEndWhitespace();
                sb.Append($"<{TagType}{GenerateMyAttributesString()}>");

                foreach (var tag in this) sb = tag.ToHtml(sb);
                    
                sb.TrimEndWhitespace();
                sb.Append($"</{TagType}>");
            }
            else
            {
                sb.TrimEndWhitespace();
                sb.Append($"<{TagType}{GenerateMyAttributesString()}></{TagType}>");
            }

            return sb;
        }
        else
        {
            return base.ToHtml(sb);
        }
    }
    #endregion

    #region Properies
    public bool GenerateInline { get; }
    #endregion
}