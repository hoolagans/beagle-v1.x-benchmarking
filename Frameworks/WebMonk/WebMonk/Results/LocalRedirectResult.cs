using System;

namespace WebMonk.Results;

public class LocalRedirectResult : RedirectResult
{
    #region Constructors
    public LocalRedirectResult(string localRedirectLocation) : base(PrepareLocalRedirectLocation(localRedirectLocation)) {}
    #endregion

    #region Protected Helper Methods
    public static string PrepareLocalRedirectLocation(string localRedirectLocation)
    {
        localRedirectLocation = localRedirectLocation.Trim();
        if (!IsLocalUrl(localRedirectLocation)) throw new ArgumentException("Must be a local url", nameof(localRedirectLocation));
        return localRedirectLocation;
    }
    protected static bool IsLocalUrl(string url)
    {
        return !string.IsNullOrEmpty(url) && (url[0] == '/' && (url.Length == 1 || url[1] != '/' && url[1] != '\\'));
            
        //This is Microsoft's version that I reworked
        //return !url.IsEmpty() &&
        //       (url[0] == '/' && (url.Length == 1 || url[1] != '/' && url[1] != '\\') || // "/" or "/foo" but not "//" or "/\"
        //        url.Length > 1 && url[0] == '~' && url[1] == '/');                         // "~/" or "~/foo"
    }
    #endregion
}