using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;
using System;
using Supermodel.Mobile.Runtime.Common.Services;
using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class MultiLineTextBoxXFModel : SingleCellWritableUIComponentForTextXFModel
{
    #region Constructors
    public MultiLineTextBoxXFModel(Keyboard keyboard) : this()
    {
        Editor.Keyboard = keyboard;
    }
    public MultiLineTextBoxXFModel()
    {
        Editor = new Editor
        {
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand,
            HeightRequest = XFormsSettings.MultiLineTextBoxCellHeight - XFormsSettings.MultiLineTextLabelHeight,
            FontSize = XFormsSettings.LabelFontSize,
            TextColor = XFormsSettings.ValueTextColor
        };
        Editor.SetBinding(Entry.TextProperty, "Text");
        Editor.PropertyChanged += (_, _) =>
        {
            if (_currentValue != Editor.Text)
            {
                _currentValue = Editor.Text;
                OnChanged?.Invoke((IBasicCRUDDetailPage)ParentPage);
            }
        };
        StackLayoutView.Orientation = StackOrientation.Vertical;
        //StackLayoutView.Padding = Pick.ForPlatform(new Thickness(8, 10), new Thickness(8, 0), new Thickness(8, 10));
        StackLayoutView.Padding = Pick.ForPlatform(new Thickness(8, 10), new Thickness(8, 0));
        StackLayoutView.Children.Add(Editor);
        SetHeight(XFormsSettings.MultiLineTextBoxCellHeight);
        StackLayoutView.HeightRequest = XFormsSettings.MultiLineTextBoxCellHeight;
        Tapped += (_, _) => Editor.Focus();
    }
    #endregion

    #region EventHandling
    public Action<IBasicCRUDDetailPage> OnChanged { get; set; }
    #endregion

    #region Properties
    public void SetHeight(int newHeight)
    {
        Height = newHeight;
        Editor.HeightRequest = ShowDisplayNameIfApplies ? newHeight - XFormsSettings.MultiLineTextLabelHeight : newHeight - 20;
    }
    public override string Text
    {
        get => Editor.Text;
        set
        {
            _currentValue = value;
            if (value == Editor.Text) return;
            Editor.Text = value;
            OnPropertyChanged();
        }
    }
    private string _currentValue;

    public Editor Editor { get; }
    public override TextAlignment TextAlignmentIfApplies { get; set; }

    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = IsEnabled = value;
            DisplayNameLabel.TextColor = value ? XFormsSettings.LabelTextColor : XFormsSettings.DisabledTextColor;
            Editor.TextColor = value ? XFormsSettings.ValueTextColor : XFormsSettings.DisabledTextColor;
            RequiredFieldIndicator.TextColor = value ? XFormsSettings.RequiredAsteriskColor : XFormsSettings.DisabledTextColor;
        }
    }
    private bool _active = true;
    #endregion
}