using Supermodel.Mobile.Runtime.Common.Services;
using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class MultiLineTextBoxReadOnlyXFModel : SingleCellReadOnlyUIComponentForTextXFModel
{
    #region Constructors
    public MultiLineTextBoxReadOnlyXFModel()
    {
        TextLabel = new Label
        {
            FontSize = XFormsSettings.LabelFontSize,
            TextColor = XFormsSettings.ValueTextColor,
            HeightRequest = XFormsSettings.MultiLineTextBoxReadOnlyCellHeight - XFormsSettings.MultiLineTextLabelHeight
        };
        ScrollView = new ScrollView{ Content = TextLabel, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            
        TextLabel.SetBinding(Entry.TextProperty, "Text");
        StackLayoutView.Orientation = StackOrientation.Vertical;
        //StackLayoutView.Padding = Pick.ForPlatform(8, new Thickness(8, 10), 8);
        StackLayoutView.Padding = Pick.ForPlatform(8, new Thickness(8, 10));
        StackLayoutView.Children.Add(ScrollView);

        SetHeight(XFormsSettings.MultiLineTextBoxReadOnlyCellHeight);
        StackLayoutView.HeightRequest = XFormsSettings.MultiLineTextBoxReadOnlyCellHeight;
    }
    #endregion

    #region Properties
    public void SetHeight(int newHeight)
    {
        Height = newHeight;
        ScrollView.HeightRequest = ShowDisplayNameIfApplies ? newHeight - XFormsSettings.MultiLineTextLabelHeight : newHeight - 20;
    }
    public override string Text
    {
        get => TextLabel.Text;
        set
        {
            if (value == TextLabel.Text) return;
            TextLabel.Text = value;
            OnPropertyChanged();
        }
    }
    public ScrollView ScrollView { get; }
    public Label TextLabel { get; }
    public override TextAlignment TextAlignmentIfApplies { get; set; }
    #endregion
}