using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Droid.App;

using Xamarin.Forms.Platform.Android;
using Android.OS;
using Supermodel.Mobile.Runtime.Common.XForms.App;

public abstract class DroidFormsApplication<TApp> : DroidFormsApplication where TApp : SupermodelXamarinFormsApp, new()
{
    protected override void OnCreate(Bundle bundle)
    {
        MainActivity = this;
        base.OnCreate(bundle);
        Forms.Init(this, bundle);
        LoadApplication(new TApp());
    }        
}
public abstract class DroidFormsApplication : FormsApplicationActivity
{
    public static FormsApplicationActivity MainActivity { get; protected set; }
}