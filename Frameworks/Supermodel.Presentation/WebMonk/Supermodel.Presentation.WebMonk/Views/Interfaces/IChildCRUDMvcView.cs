using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Views.Interfaces;

public interface IChildCRUDMvcView<in TChildDetailMvcModel>
    where TChildDetailMvcModel : class, IChildMvcModelForEntity, new()
{
    IGenerateHtml RenderDetail(TChildDetailMvcModel model);
}