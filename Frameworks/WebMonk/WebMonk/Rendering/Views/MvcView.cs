using WebMonk.Context;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace WebMonk.Rendering.Views;

public abstract class MvcView
{
    #region Constructors
    protected MvcView()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Layout = GetLayout();
    }
    #endregion
        
    #region Layout Methods
    protected IMvcLayout? Layout { get; set; }
    protected virtual IMvcLayout? GetLayout() { return HttpContext.Current.WebServer.DefaultLayout; }

    protected virtual IGenerateHtml ApplyToDefaultLayout(IGenerateHtml tags)
    {
        var layout = Layout?.RenderDefaultLayout();
        return ApplyToLayout(layout, tags);
    }
    protected static IGenerateHtml ApplyToLayout(IGenerateHtml? layout, IGenerateHtml tags)
    {
        if (layout == null) return tags;
        return layout.FillBodySectionWith(tags);
    }
    #endregion
}