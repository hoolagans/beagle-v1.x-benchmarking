using System;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public abstract class SingleCellWritableUIComponentWithoutBackingXFModel : SingleCellReadOnlyUIComponentWithoutBackingXFModel, IWritableUIComponentXFModel
{
    #region Constructors
    protected SingleCellWritableUIComponentWithoutBackingXFModel()
    {
        ValidationErrorIndicator = new Button{ Text = "!", TextColor = Color.Red };
        ValidationErrorIndicator.Clicked += ValidationIndicatorClicked;

        RequiredFieldIndicator = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = XFormsSettings.RequiredAsteriskColor, Text = "*" };
    }
    #endregion

    #region Event Handlers
    public async void ValidationIndicatorClicked(object sender, EventArgs args)
    {
        if (ParentPage != null) await ParentPage.DisplayAlert("", ErrorMessage, "Ok");
    }
    #endregion

    #region Properties
    public Button ValidationErrorIndicator { get; set; }
    public Label RequiredFieldIndicator { get; set; }
    public bool Required
    {
        get => _required;
        set
        {
            if (!_required && value) LabelView.Children.Insert(1, RequiredFieldIndicator);
            if (_required && !value) LabelView.Children.Remove(RequiredFieldIndicator);
            _required = value;
        }
    }
    private bool _required;
        
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage == null & value != null) StackLayoutView.Children.Add(ValidationErrorIndicator);
            if (_errorMessage != null & value == null)
            {
                //this throws NullReferenceException on Android, ignore it
                try { StackLayoutView.Children.Remove(ValidationErrorIndicator); } catch (NullReferenceException) { }
            }
            _errorMessage = value;
        }
    }
    private string _errorMessage;

    public abstract object WrappedValue { get; }
    #endregion    
}