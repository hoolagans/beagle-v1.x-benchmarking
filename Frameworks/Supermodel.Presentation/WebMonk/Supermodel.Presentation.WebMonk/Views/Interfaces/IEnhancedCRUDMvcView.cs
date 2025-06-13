using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Views.Interfaces;

public interface IEnhancedCRUDMvcView<in TDetailMvcModel, TListMvcModel, TSearchMvcModel> : ICRUDMvcView<TDetailMvcModel, TListMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
{ 
    IGenerateHtml RenderSearch(TSearchMvcModel model);
    IGenerateHtml RenderList(ListWithCriteria<TListMvcModel, TSearchMvcModel> models, int totalCount);
}