using System;
using Supermodel.Persistence.DataContext;
using Supermodel.Persistence.Entities;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.HttpRequestHandlers.Controllers;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Controllers.Mvc;
#pragma warning disable 618
public abstract class InlineChildCRUDMvcController<TChildEntity, TChildDetailMvcModel, TParentEntity, TParentController, TDataContext> : ChildCRUDMvcController<TChildEntity, TChildDetailMvcModel, TParentEntity, TParentController, InlineChildCRUDMvcView<TChildEntity, TChildDetailMvcModel, TParentEntity>, TDataContext>
    where TChildEntity : class, IEntity, new()
    where TParentEntity : class, IEntity, new()
    where TChildDetailMvcModel : class, IChildMvcModelForEntity<TChildEntity, TParentEntity>, new()
    where TParentController : MvcController
    where TDataContext : class, IDataContext, new();
#pragma warning restore 618

#region This is just for use with InlineChildCRUDMvcController
[Obsolete("This class is only meant to be used inside the Supermodel Framework but cannot be made internal")]
public class InlineChildCRUDMvcView<TChildEntity, TChildDetailMvcModel, TParentEntity> : IChildCRUDMvcView<TChildDetailMvcModel>
    where TChildEntity : class, IEntity, new()
    where TParentEntity : class, IEntity, new()
    where TChildDetailMvcModel : class, IChildMvcModelForEntity<TChildEntity, TParentEntity>, new()
{
    public IGenerateHtml RenderDetail(TChildDetailMvcModel model)
    {
        throw new InvalidOperationException("InlineChildCRUDMvcController cannot call Detail View");
    }
}
#endregion