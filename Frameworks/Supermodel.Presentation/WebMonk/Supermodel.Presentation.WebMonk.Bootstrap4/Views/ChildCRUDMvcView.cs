using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Views;

public abstract class ChildCRUDMvcView<TChildDetailMvcModel> : ChildCRUDMvcViewBase<TChildDetailMvcModel>
    where TChildDetailMvcModel : class, IChildMvcModelForEntity, new()
{
    #region View Methods
    public override IGenerateHtml RenderDetail(TChildDetailMvcModel model)
    {
        var detailPageTitle = DetailPageTitle;
        if (detailPageTitle == null)
        {
            if (model.IsNewModel()) detailPageTitle = "Create New";
            else detailPageTitle = model.Label;
        }
            
        var childrenTags = RenderChildren(model);
        var editTags = new Bs4.CRUDEdit(model, detailPageTitle, ReadOnly);

        if (childrenTags == null) return ApplyToDefaultLayout(editTags);
        else return ApplyToDefaultLayout(new Tags{ editTags, childrenTags });
    }
    #endregion

    #region Overrides
    protected virtual IGenerateHtml? RenderChildren(TChildDetailMvcModel model) => null;

    protected virtual string? DetailPageTitle { get; } = null;
    protected virtual bool ReadOnly { get; } = false;
    #endregion
}