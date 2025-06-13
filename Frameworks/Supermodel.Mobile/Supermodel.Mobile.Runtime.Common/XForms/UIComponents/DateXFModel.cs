using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;
using System;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using Xamarin.Forms;    
using Supermodel.ReflectionMapper;
using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class DateXFModel : SingleCellWritableUIComponentWithoutBackingXFModel, IRMapperCustom
{
    #region Constructors
    public DateXFModel()
    {
        DatePicker = new ExtDatePicker{ HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Border = false, TextAlignment = TextAlignment.End, FontSize = XFormsSettings.LabelFontSize, TextColor = XFormsSettings.ValueTextColor };
        DatePicker.PropertyChanged += (_, _) =>
        {
            if (_currentValue != DatePicker.Date)
            {
                _currentValue = DatePicker.Date;
                OnChanged?.Invoke((IBasicCRUDDetailPage)ParentPage);
            }
        };

        StackLayoutView.Children.Add(DatePicker);
        Tapped += (_, _) => DatePicker.Focus();
    }
    #endregion

    #region ICstomMapper implementations
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
            
        if (other == null) Value = null;
        else Value = (DateTime)(object)other; 

        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        if (typeof(T) == typeof(DateTime) && Value == null) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1} because {0} is null but {1} is not nullable", GetType().Name, typeof(T).Name));
        other = (T)(object)Value; //This assignment does not do anything but we still do it for consistency
        return Task.FromResult(other);
    }
    #endregion

    #region EventHandling
    public Action<IBasicCRUDDetailPage> OnChanged { get; set; }
    #endregion

    #region Properties
    public DateTime? Value
    {
        get => DatePicker.Date;
        set
        {
            _currentValue = value;
            if (value == null) DatePicker.Date = DateTime.Today;
            else DatePicker.Date = value.Value;
        }
    }
    private DateTime? _currentValue;

    public ExtDatePicker DatePicker { get; }

    public override object WrappedValue => Value;

    public override TextAlignment TextAlignmentIfApplies
    {
        get => DatePicker.TextAlignment;
        set => DatePicker.TextAlignment = value;
    }

    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = IsEnabled = value;
            DisplayNameLabel.TextColor = value ? XFormsSettings.LabelTextColor : XFormsSettings.DisabledTextColor;
            DatePicker.TextColor = value ? XFormsSettings.ValueTextColor : XFormsSettings.DisabledTextColor;
            RequiredFieldIndicator.TextColor = value ? XFormsSettings.RequiredAsteriskColor : XFormsSettings.DisabledTextColor;
        }
    }
    private bool _active = true;
    #endregion
}