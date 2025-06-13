using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.Rendering.Templates;

public interface IEditorTemplate
{
    IGenerateHtml EditorTemplate(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue, object? attributes = null);    
}