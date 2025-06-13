using Supermodel.ReflectionMapper;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Newtonsoft.Json;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.Mobile.Runtime.Common.Models;

public abstract class ChildModel: ISupermodelListTemplate
{
    #region Overrides
    [JsonIgnore, NotRCompared] public virtual Guid[] ParentGuidIdentities { get; set; }
    [JsonIgnore, NotRCompared] public virtual Guid ChildGuidIdentity { get; set; } = Guid.NewGuid();

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
                var deleteAction = new MenuItem { Text = "Delete", IsDestructive = true, Parent = cell };
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
    // ReSharper disable once UnusedParameter.Global
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
}