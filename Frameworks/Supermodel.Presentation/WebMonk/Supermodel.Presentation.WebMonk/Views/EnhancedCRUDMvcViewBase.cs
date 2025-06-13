using System;
using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Views;

public abstract class EnhancedCRUDMvcViewBase<TMvcModel, TSearchMvcModel> : EnhancedCRUDMvcViewBase<TMvcModel, TMvcModel, TSearchMvcModel>
    where TMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new();

public abstract class EnhancedCRUDMvcViewBase<TDetailMvcModel, TListMvcModel, TSearchMvcModel> : CRUDMvcViewBase<TDetailMvcModel, TListMvcModel>, IEnhancedCRUDMvcView<TDetailMvcModel, TListMvcModel, TSearchMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
{
    public abstract IGenerateHtml RenderSearch(TSearchMvcModel model);

    public override IGenerateHtml RenderList(List<TListMvcModel> models) { throw new InvalidOperationException(); }
    public abstract IGenerateHtml RenderList(ListWithCriteria<TListMvcModel, TSearchMvcModel> models, int totalCount);
}