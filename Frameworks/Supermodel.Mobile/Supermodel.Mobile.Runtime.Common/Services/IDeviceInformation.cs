namespace Supermodel.Mobile.Runtime.Common.Services;

[SharedService.Singleton]
//[SharedService.ImplementedBy("Supermodel.Mobile.Runtime.iOS.Services.DeviceInformation, Supermodel.Mobile.Runtime.iOS", "Supermodel.Mobile.Runtime.Droid.Services.DeviceInformation, Supermodel.Mobile.Runtime.Droid")]
public interface IDeviceInformation
{
    bool IsRunningOnEmulator();
    bool? IsJailbroken(bool returnNullIfNotSupported);
    bool? IsDeviceSecuredByPasscode(bool returnNullIfNotSupported);
}