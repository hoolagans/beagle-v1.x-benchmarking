using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using System;
using System.Collections.ObjectModel;
using Supermodel.ReflectionMapper;
using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

public abstract class CRUDChildDetailPage<TModel, TChildModel, TXFModel, TDataContext> : CRUDDetailPageBase<TModel, TXFModel, TDataContext>
    where TModel : class, ISupermodelNotifyPropertyChanged, IModel, new()
    where TChildModel : ChildModel, new()
    where TXFModel : XFModelForChildModel<TModel, TChildModel>, new()
    where TDataContext : class, IDataContext, new()
{
    #region Initializers
    public virtual async Task<CRUDChildDetailPage<TModel, TChildModel, TXFModel, TDataContext>> InitAsync(ObservableCollection<TModel> models, string title, TModel model, Guid childGuidIdentity, params Guid[] parentGuidIdentities)
    {
        var xfModel = new TXFModel();
        await xfModel.InitAsync(model, childGuidIdentity, parentGuidIdentities);
        xfModel = await xfModel.MapFromAsync(model);

        var originalXFModel = new TXFModel();
        await originalXFModel.InitAsync(model, childGuidIdentity, parentGuidIdentities);
        originalXFModel = await originalXFModel.MapFromAsync(model);

        XFModel = xfModel;
        OriginalXFModel = originalXFModel;

        return (CRUDChildDetailPage<TModel, TChildModel, TXFModel, TDataContext>)await base.InitAsync(models, title, model, xfModel, originalXFModel);
    }
    #endregion

    #region Overrides
    protected override async Task<TXFModel> GetBlankXFModelAsync()
    {
        var blankXfModel = (TXFModel)await new TXFModel().InitAsync(Model, OriginalXFModel.ChildGuidIdentity, OriginalXFModel.ParentGuidIdentities);
        return blankXfModel;
    }
    #endregion
}