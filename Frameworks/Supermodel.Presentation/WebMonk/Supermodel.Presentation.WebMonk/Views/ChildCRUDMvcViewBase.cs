using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Views;

public abstract class ChildCRUDMvcViewBase<TChildDetailMvcModel> : MvcView, IChildCRUDMvcView<TChildDetailMvcModel>
    where TChildDetailMvcModel : class, IChildMvcModelForEntity, new()
{
    public abstract IGenerateHtml RenderDetail(TChildDetailMvcModel model);
}