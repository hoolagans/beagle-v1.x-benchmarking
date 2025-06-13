using Supermodel.Mobile.Runtime.Common.XForms.Pages.CRUDDetail;
using System;
using System.Collections.Generic;
using Supermodel.Mobile.Runtime.Common.XForms.ViewModels;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class ButtonXFModel : ViewCell, ISupermodelMobileDetailTemplate
{
    #region Constructors
    public ButtonXFModel(string text, Action<IBasicCRUDDetailPage> onClicked) : this(text, null, onClicked) {}
    public ButtonXFModel(string text, string imageFileName, Action<IBasicCRUDDetailPage> onClicked) : this(text, imageFileName, imageFileName, onClicked) { }
    public ButtonXFModel(string text, string imageFileName, string disabledImageFileName, Action<IBasicCRUDDetailPage> onClicked)
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
            ImageFileName = imageFileName;
            DisabledImageFileName = disabledImageFileName;

            Image = new Image { Source = ImageFileName };
            StackLayoutView.Children.Add(Image);
        }

        Label = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, Text = text, FontSize = XFormsSettings.LabelFontSize };
        if (Device.RuntimePlatform == Device.iOS) Label.TextColor = Color.FromHex("#007AFF");
        StackLayoutView.Children.Add(Label);

        OnClicked = onClicked;

        Tapped += (_, _) => { OnClicked?.Invoke((IBasicCRUDDetailPage)ParentPage); };
    }
    #endregion

    #region ISupermodelMobileDetailTemplate implemetation
    public List<Cell> RenderDetail(Page parentPage, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        ParentPage = parentPage;
        return new List<Cell> { this };
    }
    #endregion

    #region EventHandling
    public Action<IBasicCRUDDetailPage> OnClicked { get; set; }
    #endregion

    #region Properties
    public bool Active
    {
        get => _active;
        set
        {
            _active = IsEnabled = value;

            if (_active)
            {
                Image.Source = ImageFileName;
                if (Device.RuntimePlatform == Device.iOS) Label.TextColor = Color.FromHex("#007AFF");
            }
            else
            {
                Image.Source = DisabledImageFileName;
                Label.TextColor = XFormsSettings.DisabledTextColor;
            }

        }
    }
    private bool _active;

    public StackLayout StackLayoutView
    {
        get => (StackLayout)View;
        set => View = value;
    }

    public string Text
    {
        get => Label.Text;
        set => Label.Text = value;
    }

    public Label Label { get; }
    public Image Image { get; }
    public string ImageFileName { get; }
    public string DisabledImageFileName { get; }

    public Page ParentPage { get; set; }
    #endregion
}