using Supermodel.DataAnnotations.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Supermodel.Mobile.Runtime.Common.Repository;
using Supermodel.Mobile.Runtime.Common.DataContext.Core;
using Supermodel.ReflectionMapper;
using System.ComponentModel;
using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.DataContext.Sqlite;
using Supermodel.Mobile.Runtime.Common.UnitOfWork;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.Mobile.Runtime.Common.Models;

public abstract class Model : IModel, ISupermodelListTemplate
{
    #region Methods
    public virtual List<TChildModel> GetChildList<TChildModel>(params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new()
    {
        throw new SupermodelException("If Model has children, you must override GetChildList<TChildModel>");
    }
    public virtual TChildModel GetChild<TChildModel>(Guid childGuidIdentity, params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new()
    {
        if (parentGuidIdentities == null) throw new ArgumentNullException(nameof(parentGuidIdentities), "Override AfterLoad() on root Model and assign parent identities for each child");
        var child = GetChildOrDefault<TChildModel>(childGuidIdentity, parentGuidIdentities);
        if (child == null) throw new InvalidOperationException("No element satisfies the condition in predicate.");
        return child;
    }
    public virtual TChildModel GetChildOrDefault<TChildModel>(Guid childGuidIdentity, params Guid[] parentGuidIdentities) where TChildModel : ChildModel, new()
    {
        if (parentGuidIdentities == null) throw new ArgumentNullException(nameof(parentGuidIdentities), "Override AfterLoad() on root Model and assign parent identities for each child");
        var child = GetChildList<TChildModel>(parentGuidIdentities).SingleOrDefault(x => x.ChildGuidIdentity == childGuidIdentity);
        return child;
    }
    public virtual void AddChild<TChildModel>(TChildModel child, int? index = null) where TChildModel : ChildModel, new()
    {
        if (child == null) throw new ArgumentNullException(nameof(child));
        if (child.ParentGuidIdentities == null) throw new ArgumentNullException(nameof(child.ParentGuidIdentities), "Override AfterLoad() on root Model and assign ParentIdentities for each child");
        if (GetChildOrDefault<TChildModel>(child.ChildGuidIdentity, child.ParentGuidIdentities) != null) throw new SupermodelException("Model.AddChild<TChildModel>(): Attempting to add a duplicate child");
        GetChildList<TChildModel>(child.ParentGuidIdentities).Add(child);
    }
    public virtual int DeleteChild<TChildModel>(TChildModel child) where TChildModel : ChildModel, new()
    {
        if (child == null) throw new ArgumentNullException(nameof(child));
        if (child.ParentGuidIdentities == null) throw new ArgumentNullException(nameof(child.ParentGuidIdentities), "Override AfterLoad() on root Model and assign ParentIdentities for each child");
        var index = GetChildList<TChildModel>(child.ParentGuidIdentities).IndexOf(child);
        if (index < 0) throw new SupermodelException("DeleteChild(): Element not found");
        GetChildList<TChildModel>(child.ParentGuidIdentities).RemoveAt(index);
        return index;
    }

    public virtual void Add()
    {
        CreateRepo().ExecuteMethod("Add", this);
    }
    public virtual void Delete()
    {
        CreateRepo().ExecuteMethod("Delete", this);
    }
    public virtual void Update()
    {
        CreateRepo().ExecuteMethod("ForceUpdate", this);
    }

    public virtual void BeforeSave(PendingAction.OperationEnum operation)
    {
        //default is doing nothing
    }
    public virtual void AfterLoad()
    {
        //default is doing nothing
    }

    public virtual object CreateRepo()
    {
        return RepoFactory.CreateForRuntimeType(GetType());
    }
    public Task<ValidationResultList> ValidateAsync(ValidationContext validationContext)
    {
        return Task.FromResult(new ValidationResultList());
    }
    #endregion

    #region ISupermodelListTemplate implemetation
    public virtual DataTemplate GetListCellDataTemplate(EventHandler selectItemHandler, EventHandler deleteItemHandler)
    {
        var dataTemplate = new DataTemplate(() =>
        {
            var cell = ReturnACell();
            //if delete item handler is not there, tap is not broken, so we don't need select item handler
            if (deleteItemHandler != null && selectItemHandler != null)
            {
                var selectAction = new MenuItem { Text = "Edit", Parent = cell };
                selectAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                cell.ContextActions.Add(selectAction);
                selectAction.Clicked += selectItemHandler;
            }
            if (deleteItemHandler != null)
            {
                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true };
                deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
                cell.ContextActions.Add(deleteAction);
                deleteAction.Clicked += deleteItemHandler;
            }
            return cell;
        });
        SetUpBindings(dataTemplate);
        return dataTemplate;
    }
    public virtual Cell ReturnACell()
    {
        var msg = $"In order to use '{GetType().Name}' class with CRUD features, you must override ReturnACell() and SetUpBindings() methods!";
        throw new SupermodelException(msg);
    }
    public virtual void SetUpBindings(DataTemplate dataTemplate)
    {
        var msg = $"In order to use '{GetType().Name}' class with CRUD features, you must override ReturnACell() and SetUpBindings() methods!";
        throw new SupermodelException(msg);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Properties
    public long Id { get; set; }
    [NotRMapped, NotRCompared, JsonIgnore] public virtual string Identity => Id.ToString();

    [NotRMapped] public DateTime? BroughtFromMasterDbOnUtc { get; set; }
    public bool ShouldSerializeBroughtFromMasterDbOnUtc()
    {
        if (UnitOfWorkContextCore.StackCount == 0) return false;
        return UnitOfWorkContextCore.CurrentDataContext is SqliteDataContext;
    }

    [JsonIgnore, NotRMapped] public virtual bool IsNew => Id == 0;
    #endregion
        
}