#nullable disable

using System;
using System.IO;
using System.Net;
using WebMonk.Context.WMHttpListenerObjects;

namespace Supermodel.Presentation.WebMonk.Batch;

public class BatchHttpListenerResponse : IHttpListenerResponse
{
    #region Constructors
    public BatchHttpListenerResponse(IHttpListenerResponse rootResponse)
    {
        StatusCode = rootResponse.StatusCode;
        Headers = new WebHeaderCollection();
        //ContentEncoding = rootResponse.ContentEncoding;
        ContentType = rootResponse.ContentType;
        Cookies = new CookieCollection();
        OutputStream = new MemoryStream();
        RedirectLocation = rootResponse.RedirectLocation;
        StatusDescription = rootResponse.StatusDescription;
        ProtocolVersion = rootResponse.ProtocolVersion;

        RootResponse = rootResponse;
    }
    #endregion

    #region IHttpListenerResponse implementation
    public void Dispose()
    {
        //do nothing
    }

    public WebHeaderCollection Headers { get; set; }
    public string ContentType 
    { 
        get => Headers["Content-Type"];
        set { if (!string.IsNullOrEmpty(value)) Headers.Set("Content-Type", value); }
    }
    public CookieCollection Cookies { get; set; }
    public Stream OutputStream { get; set; }
    public string RedirectLocation { get; set; }
    public string StatusDescription { get; set; }
        
    public void AddHeader(string name, string value)
    {
        Headers.Set(name, value);
    }
    public void AppendHeader(string name, string value)
    {
        Headers.Add(name, value);
    }

    public void AppendCookie(Cookie cookie)
    {
        RootResponse.AppendCookie(cookie);
        //Cookies.Add(cookie);
    }
    public void SetCookie(Cookie cookie)
    {
        RootResponse.SetCookie(cookie);
        //if (cookie == null) throw new ArgumentNullException(nameof(cookie));

        //_cloneFunc ??= (Func<Cookie, Cookie>) typeof(Cookie).GetMethod("Clone", BindingFlags.NonPublic)!.CreateDelegate(typeof(Func<Cookie, Cookie>));
        //var newCookie = _cloneFunc(cookie);

        //_internalAddFunc ??= (Func<CookieCollection, Cookie, bool, int>)typeof(CookieCollection).GetMethod("InternalAdd", BindingFlags.NonPublic)!.CreateDelegate(typeof(Func<CookieCollection, Cookie, bool, int>));
        //var added = _internalAddFunc(Cookies, newCookie, true);
            
        //if (added != 1) throw new SupermodelException("Cookie exists");
    }

    public int StatusCode { get; set; }
    public Version ProtocolVersion { get; set; }
    public void Close()
    {
        //do nothing
    }
    #endregion

    #region Private Properties
    protected IHttpListenerResponse RootResponse { get; }
    //private Func<Cookie, Cookie> _cloneFunc;
    //private Func<CookieCollection, Cookie, bool, int> _internalAddFunc;
    #endregion
}