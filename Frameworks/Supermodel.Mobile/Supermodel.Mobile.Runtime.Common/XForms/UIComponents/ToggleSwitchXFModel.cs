using System.Threading.Tasks; 
using System;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.ReflectionMapper;
using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;
 
namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class ToggleSwitchXFModel : SingleCellWritableUIComponentXFModel
{
    #region Constructors
    public ToggleSwitchXFModel()
    {
        Switch = new Switch { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center, OnColor = XFormsSettings.SwitchOnColor };
        Switch.SetBinding(Switch.IsToggledProperty, "IsToggled");
        Switch.PropertyChanged += (_, _) =>
        {
            if (_currentValue != Switch.IsToggled)
            {
                _currentValue = Switch.IsToggled;
                OnChanged?.Invoke((IBasicCRUDDetailPage)ParentPage);
            }
        };
        StackLayoutView.Children.Add(Switch);
        Tapped += (_, _) =>
        {
            Switch.Focus();
            Switch.IsToggled = !Switch.IsToggled; //Surenra's change
        };
    }
    #endregion

    #region ICustomMapper implemtation
    public override Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?))
        {
            throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        }

        if (other is bool) IsToggled = (bool)(object)other;
        else IsToggled = (bool?)(object)other ?? false;

        return Task.CompletedTask;
    }
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?))
        {
            throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        }
            
        return Task.FromResult((T)(object)IsToggled);
    }
    #endregion

    #region EventHandling
    public Action<IBasicCRUDDetailPage> OnChanged { get; set; }
    #endregion

    #region Properties
    public bool IsToggled
    {
        get => Switch.IsToggled;
        set
        {
            _currentValue = value;
            if (value == Switch.IsToggled) return;
            Switch.IsToggled = value;
            OnPropertyChanged();
        }
    }
    private bool _currentValue;
    public Switch Switch { get; }

    public override object WrappedValue => Switch.IsToggled;

    public override TextAlignment TextAlignmentIfApplies
    {
        get
        {
            switch(Switch.HorizontalOptions.Alignment)
            {
                case LayoutAlignment.Start: return TextAlignment.Start;
                case LayoutAlignment.Center: return TextAlignment.Center;
                case LayoutAlignment.End: return TextAlignment.End;
                default: throw new SupermodelException("Invalid value for Switch.HorizontalOptions.Alignment. This should never happen");
            }
        }
        set
        {
            switch(value)
            {
                case TextAlignment.Start: 
                    Switch.HorizontalOptions = LayoutOptions.StartAndExpand;
                    break;
                case TextAlignment.Center: 
                    Switch.HorizontalOptions = LayoutOptions.CenterAndExpand;
                    break;
                case TextAlignment.End: 
                    Switch.HorizontalOptions = LayoutOptions.EndAndExpand;
                    break;
                default: throw new SupermodelException("Invalid value for TextAlignmentIfApplies. This should never happen");
            }
        }
    }

    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = IsEnabled = value;
            DisplayNameLabel.TextColor = value ? XFormsSettings.LabelTextColor : XFormsSettings.DisabledTextColor;
            Switch.OnColor = value ? XFormsSettings.SwitchOnColor : XFormsSettings.DisabledTextColor;
            RequiredFieldIndicator.TextColor = value ? XFormsSettings.RequiredAsteriskColor : XFormsSettings.DisabledTextColor;
        }
    }
    private bool _active = true;
    #endregion
}