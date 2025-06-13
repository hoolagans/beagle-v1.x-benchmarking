using Java.IO;
using Android.OS;
using Android.App;
using Supermodel.Mobile.Runtime.Common.Services;
using Xamarin.Essentials;

namespace Supermodel.Mobile.Runtime.Droid.Services;

public class DeviceInformation : IDeviceInformation
{
    #region IDeviceInformation implementation
    public bool? IsDeviceSecuredByPasscode(bool returnNullIfNotSupported)
    {
        if (IsRunningOnEmulator()) return null; //if we are not running on physical hardware return null (unknown)

        var result = false;

        using (var km = (KeyguardManager) Application.Context.GetSystemService(Android.Content.Context.KeyguardService))
        {
            // ReSharper disable once PossibleNullReferenceException
            if (km.IsKeyguardSecure) result = true;
        }

        return result;        
    }
    public bool? IsJailbroken(bool returnNullIfNotSupported)
    {
        if (IsRunningOnEmulator()) return null; //if we are not running on physical hardware return null (unknown)

        // ReSharper disable once ConvertToConstant.Local
        var result = false;

        var buildTags = Build.Tags;
        if (buildTags != null && buildTags.Contains("test-keys")) result = true;

        if (new File("/system/app/Superuser.apk").Exists()) result = true;

        var paths = new[] { "/sbin/su", "/system/bin/su", "/system/xbin/su", "/data/local/xbin/su", "/data/local/bin/su", "/system/sd/xbin/su", "/system/bin/failsafe/su", "/data/local/su" };
        foreach(var path in paths) 
        {
            if (new File(path).Exists()) result = true;
        }

        try
        {
            // ReSharper disable once PossibleNullReferenceException
            using (var process = Java.Lang.Runtime.GetRuntime().Exec(new [] { "/system/xbin/which", "su" }))
            {
                // ReSharper disable once PossibleNullReferenceException
                var br = new BufferedReader(new InputStreamReader(process.InputStream));
                if (br.ReadLine() != null) result = true;
            }
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch{}

        return result;        
    }
    public bool IsRunningOnEmulator() => DeviceInfo.DeviceType == DeviceType.Virtual;
    #endregion
}