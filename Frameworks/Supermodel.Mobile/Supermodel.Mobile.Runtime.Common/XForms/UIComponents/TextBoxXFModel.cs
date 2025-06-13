using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using System;
using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class TextBoxXFModel : SingleCellWritableUIComponentForTextXFModel
{
    #region Constructors
    public TextBoxXFModel(Keyboard keyboard):this()
    {
        TextEntry.Keyboard = keyboard;
    }
    public TextBoxXFModel()
    {
        TextEntry = new ExtEntry { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Border = false, TextAlignment = TextAlignment.End, WidthRequest = 1, FontSize = XFormsSettings.LabelFontSize, TextColor = XFormsSettings.ValueTextColor };
        TextEntry.SetBinding(Entry.TextProperty, "Text");
        TextEntry.PropertyChanged += (_, _) =>
        {
            if (_currentValue != TextEntry.Text)
            {
                _currentValue = TextEntry.Text;
                OnChanged?.Invoke((IBasicCRUDDetailPage)ParentPage);
            }
        };
        StackLayoutView.Children.Add(TextEntry);
        Tapped += (_, _) => TextEntry.Focus();
    }
    #endregion

    #region EventHandling
    public Action<IBasicCRUDDetailPage> OnChanged { get; set; }
    #endregion

    #region Properties
    public override string Text
    {
        get => TextEntry.Text;
        set
        {
            _currentValue = value;
            if (value == TextEntry.Text) return;
            TextEntry.Text = value;
            OnPropertyChanged();
        }
    }
    private string _currentValue;

    public ExtEntry TextEntry { get; }
    public override TextAlignment TextAlignmentIfApplies
    {
        get => TextEntry.TextAlignment;
        set => TextEntry.TextAlignment = value;
    }

    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = IsEnabled = value;
            DisplayNameLabel.TextColor = value ? XFormsSettings.LabelTextColor : XFormsSettings.DisabledTextColor;
            TextEntry.TextColor = value ? XFormsSettings.ValueTextColor : XFormsSettings.DisabledTextColor;
            RequiredFieldIndicator.TextColor = value ? XFormsSettings.RequiredAsteriskColor : XFormsSettings.DisabledTextColor;
        }
    }
    private bool _active = true;
    #endregion
}