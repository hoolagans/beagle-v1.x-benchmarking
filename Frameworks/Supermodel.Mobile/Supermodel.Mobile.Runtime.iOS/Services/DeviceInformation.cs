using Foundation;
using Security;
using Supermodel.Mobile.Runtime.Common.Services;
using System;
using System.IO;
using UIKit;
using Xamarin.Essentials;

namespace Supermodel.Mobile.Runtime.iOS.Services;

public class DeviceInformation : IDeviceInformation
{
    #region IDeviceInformation implementation
    public bool? IsDeviceSecuredByPasscode(bool returnNullIfNotSupported)
    {
        if (IsRunningOnEmulator()) return null; //if we are not running on physical hardware return null (unknown)

        var result = false;
            
        const string text = "Supermodel.Mobile Passcode Test";
        var record = new SecRecord(SecKind.GenericPassword) { Generic = NSData.FromString (text), Accessible = SecAccessible.WhenPasscodeSetThisDeviceOnly };
        var status = SecKeyChain.Add(record);
        if (status == SecStatusCode.Success || status == SecStatusCode.DuplicateItem)
        {
            result = true;
            SecKeyChain.Remove(record);
        }

        return result;        
    }
    public bool? IsJailbroken(bool returnNullIfNotSupported)
    {
        if (IsRunningOnEmulator()) return null; //if we are not running on physical hardware return null (unknown)

        var result = false;

        var paths = new [] {@"/Applications/Cydia.app", @"/Library/MobileSubstrate/MobileSubstrate.dylib", @"/bin/bash", @"/usr/sbin/sshd", @"/etc/apt" };
        foreach (var path in paths)
        {
            if (NSFileManager.DefaultManager.FileExists(path)) result = true;
        }

        try
        {
            const string filename = @"/private/jailbreak.txt";
            File.WriteAllText(filename, "This is a test.");
            result = true;
            File.Delete(filename);
        }
        // ReSharper disable once EmptyGeneralCatchClause
        catch (Exception){} //if exception is thrown, we are not jailbroken

        if (UIApplication.SharedApplication.CanOpenUrl(new NSUrl("cydia://package/com.exapmle.package"))) result = true;
        return result;
    }
    public bool IsRunningOnEmulator() => DeviceInfo.DeviceType == DeviceType.Virtual;
    #endregion
}