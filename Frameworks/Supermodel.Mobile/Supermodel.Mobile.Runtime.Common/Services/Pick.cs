using System;
using System.Reflection;
using System.Runtime.Versioning;
using Supermodel.DataAnnotations.Exceptions;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.Services;

public static class Pick
{
    public static Platform RunningPlatform()
    {
        return ForPlatform(Platform.IOS, Platform.Droid, Platform.DotNetCore);
    }
        
    public static T ForPlatform<T>(T iOS, T droid)
    {
        if (Device.RuntimePlatform == Device.iOS) return iOS;
        if (Device.RuntimePlatform == Device.Android) return droid;
        else throw new SupermodelException($"Unsupported Platform {Device.RuntimePlatform}");
    }

    public static T ForPlatform<T>(T iOS, T droid, T netCore)
    {
        var framework = Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;
        if (framework != null && framework.StartsWith(".NETCoreApp", StringComparison.Ordinal)) return netCore;
        else return ForPlatform(iOS, droid);
    }
}