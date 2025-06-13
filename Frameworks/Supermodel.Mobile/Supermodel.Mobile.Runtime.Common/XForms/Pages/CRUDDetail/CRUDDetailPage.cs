using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Supermodel.ReflectionMapper;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

public abstract class CRUDDetailPage<TModel, TXFModel, TDataContext> : CRUDDetailPageBase<TModel, TXFModel, TDataContext>
    where TModel : class, ISupermodelNotifyPropertyChanged, IModel, new()
    where TXFModel : XFModelForModel<TModel>, new()
    where TDataContext : class, IDataContext, new()
{
    #region Initializers
    public virtual async Task<CRUDDetailPage<TModel, TXFModel, TDataContext>> InitAsync(ObservableCollection<TModel> models, string title, TModel model)
    {
        var xfModel = new TXFModel();
        await xfModel.InitAsync(model);
        xfModel = await xfModel.MapFromAsync(model);

        var originalXFModel = new TXFModel();
        await originalXFModel.InitAsync(model);
        originalXFModel = await originalXFModel.MapFromAsync(model);

        return (CRUDDetailPage<TModel, TXFModel, TDataContext>)await base.InitAsync(models, title, model, xfModel, originalXFModel);
    }
    #endregion

    #region Overrides
    protected override async Task<TXFModel> GetBlankXFModelAsync()
    {
        var blankModel = new TModel();
        var blankXfModel = (TXFModel) await new TXFModel().InitAsync(blankModel);
        return blankXfModel;
    }
    #endregion
}