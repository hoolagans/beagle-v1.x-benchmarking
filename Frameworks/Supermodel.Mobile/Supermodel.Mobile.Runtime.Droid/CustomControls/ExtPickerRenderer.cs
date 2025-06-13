using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Views;
using Supermodel.Mobile.Runtime.Common.XForms.UIComponents.CustomControls;
using Supermodel.Mobile.Runtime.Droid.CustomControls;
using Android.Content;

[assembly: ExportRenderer (typeof (ExtPicker), typeof (ExtPickerRenderer))]
namespace Supermodel.Mobile.Runtime.Droid.CustomControls;

public class ExtPickerRenderer : PickerRenderer
{
    #region Constructors
    public ExtPickerRenderer(Context context) : base(context){}
    #endregion
        
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
            //Android elements are already borderless by default
            var element = (ExtPicker)Element;
            //Control.SetTextColor(element.TextColor.ToAndroid());
            switch (element.TextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Start: Control.Gravity = GravityFlags.Left; break;
                case Xamarin.Forms.TextAlignment.End: Control.Gravity = GravityFlags.Right; break;
                case Xamarin.Forms.TextAlignment.Center: Control.Gravity = GravityFlags.Center; break;
            }
        }
    }
    #endregion
}