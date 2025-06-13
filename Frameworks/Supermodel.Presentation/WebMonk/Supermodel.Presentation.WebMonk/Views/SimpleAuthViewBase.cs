using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Views;

public abstract class SimpleAuthViewBase<TLoginMvcModel> : MvcView where TLoginMvcModel : class, ILoginMvcModel, new()
{
    public abstract IGenerateHtml RenderLogin(TLoginMvcModel model);
}