using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace Supermodel.Presentation.WebMonk.Views;

public abstract class CRUDMvcViewBase<TMvcModel> : CRUDMvcViewBase<TMvcModel, TMvcModel>
    where TMvcModel : class, IMvcModelForEntity, new();
    
public abstract class CRUDMvcViewBase<TDetailMvcModel, TListMvcModel> : MvcView, ICRUDMvcView<TDetailMvcModel, TListMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
{
    public abstract ListMode ListMode { get; }
        
    public abstract IGenerateHtml RenderList(List<TListMvcModel> models);
    public abstract IGenerateHtml RenderDetail(TDetailMvcModel model);
}