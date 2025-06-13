using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;
using Supermodel.Mobile.Runtime.iOS.CustomControls;

[assembly: ExportRenderer (typeof (ExtPicker), typeof (ExtPickerRenderer))]
namespace Supermodel.Mobile.Runtime.iOS.CustomControls;

public class ExtPickerRenderer : PickerRenderer
{
    #region Overrides
    protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs propertyChangedEventArgs)
    {
        base.OnElementPropertyChanged(sender, propertyChangedEventArgs);
        UpdateControl();
    }
    protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
    {
        base.OnElementChanged(e);
        UpdateControl();
    }
    #endregion

    #region Methods
    protected void UpdateControl()
    {
        if (Control != null && Element != null)
        {
            Control.BorderStyle = UITextBorderStyle.None;
            var element = (ExtPicker)Element;
            //Control.TextColor = element.TextColor.ToUIColor();
            switch (element.TextAlignment)
            {
                case TextAlignment.Start: Control.TextAlignment = UITextAlignment.Left; break;
                case TextAlignment.End: Control.TextAlignment = UITextAlignment.Right; break;
                case TextAlignment.Center: Control.TextAlignment = UITextAlignment.Center; break;
            }
        }
    }
    #endregion
}