using System.Threading.Tasks;
using Xamarin.Forms;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class TextBoxReadOnlyXFModel : SingleCellReadOnlyUIComponentForTextXFModel
{
    #region Constructors
    public TextBoxReadOnlyXFModel()
    {
        TextLabel = new Label { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center, LineBreakMode = LineBreakMode.TailTruncation, FontSize = XFormsSettings.LabelFontSize, TextColor = XFormsSettings.ValueTextColor };
        StackLayoutView.Children.Add(TextLabel);
    }
    #endregion

    #region ICustomMapper implemtation
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        return Task.FromResult(other);
    }
    #endregion

    #region Properties
    public override string Text
    {
        get => TextLabel.Text;
        set => TextLabel.Text = value;
    }
    public Label TextLabel { get; }
    public override TextAlignment TextAlignmentIfApplies
    {
        get => TextLabel.HorizontalTextAlignment;
        set => TextLabel.HorizontalTextAlignment = value;
    }
    #endregion
}