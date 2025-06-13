using Supermodel.DataAnnotations;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.RazorSharp.HtmlTags;

public class Comment : Tag
{
    #region Constructors
    public Comment(string commentBody) : base("<!--") 
    { 
        CommentBody = commentBody;
    }
    #endregion

    #region Override
    public override StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();

        sb.AppendLineIndentPlus("<!--");
        sb.AppendLine(CommentBody);
        sb.AppendLineIndentMinus("-->");

        return sb;
    }
    #endregion

    #region Properties
    public string CommentBody { get; set; }
    #endregion
}