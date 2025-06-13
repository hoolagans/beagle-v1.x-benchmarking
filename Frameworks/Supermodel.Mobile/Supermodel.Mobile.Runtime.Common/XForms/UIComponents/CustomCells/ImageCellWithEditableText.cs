using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomCells;

public class ImageCellWithEditableText : ViewCell
{
    #region Contructors
    public ImageCellWithEditableText()
    {
        View = new StackLayout
        {
            Padding = new Thickness(8, 0, 8, 0), 
            Orientation = StackOrientation.Horizontal, 
            VerticalOptions = LayoutOptions.CenterAndExpand, 
            HorizontalOptions = LayoutOptions.FillAndExpand,
            HeightRequest = 40,
        };
            
        Image = new Image { HeightRequest = 40, Aspect = Aspect.AspectFit };
            
        TextEntry = new ExtEntry { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Border = false, TextAlignment = TextAlignment.End, WidthRequest = 1, FontSize = XFormsSettings.LabelFontSize, TextColor = XFormsSettings.ValueTextColor };
        TextEntry.SetBinding(Entry.TextProperty, "Text");
            
        StackLayoutView.Children.Add(Image);
        StackLayoutView.Children.Add(TextEntry);
    }
    #endregion

    #region Properties
    public StackLayout StackLayoutView
    {
        get => (StackLayout)View;
        set => View = value;
    }
        
    public ImageSource ImageSource
    {
        get => Image.Source;
        set => Image.Source = value;
    }
    public Image Image { get; set; }

    public virtual string Text
    {
        get => TextEntry.Text;
        set
        {
            if (value == TextEntry.Text) return;
            TextEntry.Text = value;
            OnPropertyChanged();
        }
    }
    public ExtEntry TextEntry { get; set; }

    public string Placeholder
    {
        get => TextEntry.Placeholder;
        set => TextEntry.Placeholder = value;
    }
    #endregion
}