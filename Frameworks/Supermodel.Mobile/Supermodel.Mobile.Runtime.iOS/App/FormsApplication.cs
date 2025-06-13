using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.iOS.App;

using Xamarin.Forms.Platform.iOS;
using UIKit;
using Foundation;
using Supermodel.Mobile.Runtime.Common.XForms.App;

// ReSharper disable once InconsistentNaming
public abstract class iOSFormsApplication<TApp> : iOSFormsApplication where TApp : SupermodelXamarinFormsApp, new()
{
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        AppDelegate = this;
        Forms.Init();
        LoadApplication(new TApp());
        return base.FinishedLaunching(app, options);
    }        
}
// ReSharper disable once InconsistentNaming
public abstract class iOSFormsApplication : FormsApplicationDelegate
{
    public static FormsApplicationDelegate AppDelegate { get; protected set; }
}