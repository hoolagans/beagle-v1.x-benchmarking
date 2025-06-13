using System;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomCells;

public class AddNewCell : ViewCell
{
    #region Constructors
    public AddNewCell(string imageFileName)
    {
        View = new StackLayout
        {
            Padding = new Thickness(8, 0, 8, 0), 
            Orientation = StackOrientation.Horizontal, 
            VerticalOptions = LayoutOptions.CenterAndExpand, 
            HorizontalOptions = LayoutOptions.FillAndExpand,
            HeightRequest = 40,
        };

        if (imageFileName != null)
        {
            AddNewImage = new Image { Source = imageFileName };
            StackLayoutView.Children.Add(AddNewImage);
        }

        AddNewLabel = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = "Add New", FontSize = XFormsSettings.LabelFontSize }; 
        if (Device.RuntimePlatform == Device.iOS) AddNewLabel.TextColor = Color.FromHex("#007AFF");
        StackLayoutView.Children.Add(AddNewLabel);

        ValidationErrorIndicator = new Button{ Text = "!", TextColor = Color.Red, HorizontalOptions = LayoutOptions.EndAndExpand };
        ValidationErrorIndicator.Clicked += ValidationIndicatorClicked;

        RequiredFieldIndicator = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.Red, Text = "*" };
    }
    #endregion

    #region Event Handlers
    public async void ValidationIndicatorClicked(object sender, EventArgs args)
    {
        if (ParentPage != null) await ParentPage.DisplayAlert("", ErrorMessage, "Ok");
    }
    #endregion

    #region Properties
    public Page ParentPage { get; set; }
        
    public StackLayout StackLayoutView
    {
        get => (StackLayout)View;
        set => View = value;
    }

    public string Text
    {
        get => AddNewLabel.Text;
        set => AddNewLabel.Text = value;
    }

    public Label AddNewLabel { get; }
    public Image AddNewImage { get; }

    public Button ValidationErrorIndicator { get; set; }
    public Label RequiredFieldIndicator { get; set; }
    public bool Required
    {
        get => _required;
        set
        {
            if (!_required && value) StackLayoutView.Children.Insert(0, RequiredFieldIndicator);
            if (_required && !value) StackLayoutView.Children.Remove(RequiredFieldIndicator);
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
            if (_errorMessage != null & value == null) StackLayoutView.Children.Remove(ValidationErrorIndicator);
            _errorMessage = value;
        }
    }
    private string _errorMessage;
    #endregion
}