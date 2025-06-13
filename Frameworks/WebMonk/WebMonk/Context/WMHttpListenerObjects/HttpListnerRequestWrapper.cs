#nullable disable

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace WebMonk.Context.WMHttpListenerObjects;

public class HttpListenerRequestWrapper : IHttpListenerRequest
{
    #region Constructors
    public HttpListenerRequestWrapper(HttpListenerRequest request)
    {
        Request = request;
    }
    #endregion

    #region IHttpListenerRequest implementation
    public CookieCollection Cookies => Request.Cookies;
    public Encoding ContentEncoding => Request.ContentEncoding;
    public string ContentType => Request.ContentType;
    public NameValueCollection QueryString => Request.QueryString;
    public string RawUrl => Request.RawUrl;
    public string UserAgent => Request.UserAgent;
    public string UserHostAddress => Request.UserHostAddress;
    public string UserHostName => Request.UserHostName;
    public Uri UrlReferrer => Request.UrlReferrer;
    public Uri Url => Request.Url;
    public Version ProtocolVersion => Request.ProtocolVersion;
    public NameValueCollection Headers => Request.Headers;
    public string HttpMethod => Request.HttpMethod;
    public Stream InputStream => Request.InputStream;
    public bool IsSecureConnection => Request.IsSecureConnection;
    public bool HasEntityBody => Request.HasEntityBody;
    public IPEndPoint RemoteEndPoint => Request.RemoteEndPoint;
    #endregion
        
    #region Properties
    protected  HttpListenerRequest Request { get; }
    #endregion
}