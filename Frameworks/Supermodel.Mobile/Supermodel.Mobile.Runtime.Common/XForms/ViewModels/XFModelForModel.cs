using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.XForms.ViewModels;

[RMCopyAllPropsShallow]
public abstract class XFModelForModel<TModel> : XFModelForModelBase<TModel> where TModel : class, IModel, ISupermodelNotifyPropertyChanged, new()
{
    #region Constructors
    public virtual Task<XFModelForModel<TModel>> InitAsync(TModel model)
    {
        Model = model;
        return Task.FromResult(this);
    }
    #endregion
}