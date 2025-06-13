using Supermodel.DataAnnotations.Validations;
using System;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using System.Collections.Generic;

namespace Supermodel.Mobile.Runtime.Common.Models;

public interface IModel : IHaveIdentity, IAsyncValidatableObject
{
    long Id { get; set; }
    bool IsNew { get; }

    DateTime? BroughtFromMasterDbOnUtc { get; set; }

    //IModel PrepareForSerializingForMasterDb();
    //IModel PrepareForSerializingForLocalDb();

    void Add();
    void Delete();
    void Update();

    void BeforeSave(PendingAction.OperationEnum operation);
    void AfterLoad();

    // ReSharper disable once UnusedMemberInSuper.Global
    // ReSharper disable UnusedParameter.Global
    List<TChildModel> GetChildList<TChildModel>(params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new();
    TChildModel GetChild<TChildModel>(Guid childGuidIdentity, params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new();
    TChildModel GetChildOrDefault<TChildModel>(Guid childGuidIdentity, params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new();
    void AddChild<TChildModel>(TChildModel child, int? index = null) where TChildModel : ChildModel, new();
    int DeleteChild<TChildModel>(TChildModel child) where TChildModel : ChildModel, new();
    // ReSharper restore UnusedParameter.Global
}