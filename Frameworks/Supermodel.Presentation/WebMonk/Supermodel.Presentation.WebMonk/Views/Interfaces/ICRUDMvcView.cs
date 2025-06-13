using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Views.Interfaces;

public interface ICRUDMvcView<in TDetailMvcModel, TListMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
{
    IGenerateHtml RenderList(List<TListMvcModel> models);
    IGenerateHtml RenderDetail(TDetailMvcModel model);

    ListMode ListMode { get; }
}