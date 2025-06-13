#nullable disable

using System;
using System.IO;
using System.Net;
using System.Text;

namespace WebMonk.Context.WMHttpListenerObjects;

public class HttpListenerResponseWrapper : IHttpListenerResponse
{
    #region Constructors
    public HttpListenerResponseWrapper(HttpListenerResponse response)
    {
        Response = response;
    }
    #endregion

    #region IHttpListenerResponse implementation
    public void Dispose()
    {
        ((IDisposable)Response).Dispose();
    }
    public WebHeaderCollection Headers
    {
        get => Response.Headers;
        set => Response.Headers = value;
    }
    public Encoding ContentEncoding
    {
        get => Response.ContentEncoding;
        set => Response.ContentEncoding = value;
    }
    public string ContentType
    {
        get => Response.ContentType;
        set => Response.ContentType = value;
    }
    public CookieCollection Cookies
    {
        get => Response.Cookies;
        set => Response.Cookies = value;
    }
    public Stream OutputStream => Response.OutputStream;
    public string RedirectLocation
    {
        get => Response.RedirectLocation;
        set => Response.RedirectLocation = value;
    }
    public string StatusDescription
    {
        get => Response.StatusDescription;
        set => Response.StatusDescription = value;
    }
    public void AddHeader(string name, string value)
    {
        Response.AddHeader(name, value);
    }
    public void AppendHeader(string name, string value)
    {
        Response.AppendHeader(name, value);
    }
    public void AppendCookie(Cookie cookie)
    {
        Response.AppendCookie(cookie);
    }
    public void SetCookie(Cookie cookie)
    {
        Response.SetCookie(cookie);
    }
    public int StatusCode
    {
        get => Response.StatusCode;
        set => Response.StatusCode = value;
    }
    public Version ProtocolVersion
    {
        get => Response.ProtocolVersion;
        set => Response.ProtocolVersion = value;
    }
    public void Close()
    {
        Response.Close();
    }
    #endregion

    #region Proprties
    protected HttpListenerResponse Response { get; }
    #endregion
}