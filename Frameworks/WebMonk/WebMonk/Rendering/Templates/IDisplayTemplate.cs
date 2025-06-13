using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.Rendering.Templates;

public interface IDisplayTemplate
{
    IGenerateHtml DisplayTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null);
}