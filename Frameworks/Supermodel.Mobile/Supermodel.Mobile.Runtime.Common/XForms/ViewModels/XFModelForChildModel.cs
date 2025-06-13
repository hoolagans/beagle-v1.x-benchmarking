using Supermodel.Mobile.Runtime.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.XForms.ViewModels;

public abstract class XFModelForChildModel<TModel, TChildModel> : XFModelForModelBase<TModel> 
    where TModel : class, IModel, ISupermodelNotifyPropertyChanged, new()
    where TChildModel : ChildModel, new()
{
    #region Constructors
    public virtual Task<XFModelForChildModel<TModel, TChildModel>> InitAsync(TModel model, Guid childGuidIdentity, params Guid[] parentGuidIdentities)
    {
        ParentGuidIdentities = parentGuidIdentities ?? throw new ArgumentNullException(nameof(parentGuidIdentities), "You may need tp override OnLoad on root Model and set up ParentIdentities for all ChildModels there");
        ChildGuidIdentity = childGuidIdentity;
        Model = model;
        return Task.FromResult(this);
    }
    #endregion

    #region Overrides
    public override Task MapFromCustomAsync<T>(T other)
    {
        var model = (TModel)(object)other;
        var childModel = model.GetChildOrDefault<TChildModel>(ChildGuidIdentity, ParentGuidIdentities) ?? new TChildModel { ChildGuidIdentity = ChildGuidIdentity, ParentGuidIdentities = ParentGuidIdentities };
        return this.MapFromCustomBaseAsync(childModel);
    }
    public override async Task<T> MapToCustomAsync<T>(T other)
    {
        var model = (TModel)(object)other;
        var childModel = model.GetChildOrDefault<TChildModel>(ChildGuidIdentity, ParentGuidIdentities);
        if (childModel == null)
        {
            childModel = new TChildModel { ParentGuidIdentities = ParentGuidIdentities };
            await this.MapToCustomBaseAsync(childModel);
            model.AddChild(childModel);
        }
        else
        {
            await this.MapToCustomBaseAsync(childModel);
        }
        return other;
    }
    #endregion

    #region Properties
    [ScaffoldColumn(false), NotRMapped, NotRCompared] public virtual Guid[] ParentGuidIdentities { get; set; }
    [ScaffoldColumn(false), NotRMapped, NotRCompared] public virtual Guid ChildGuidIdentity { get; set; } = Guid.NewGuid();
    #endregion
}