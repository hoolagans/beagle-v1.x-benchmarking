using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.Rendering.Views;

public interface IMvcLayout
{
    IGenerateHtml RenderDefaultLayout();
}