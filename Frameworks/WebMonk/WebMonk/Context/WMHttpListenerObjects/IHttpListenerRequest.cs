#nullable disable

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace WebMonk.Context.WMHttpListenerObjects;

public interface IHttpListenerRequest
{
    //string[] AcceptTypes { get; }
    //string[] UserLanguages { get; }
    CookieCollection Cookies { get; }
    Encoding ContentEncoding { get; }
    string ContentType { get; }
    //bool IsLocal { get; }
    //bool IsWebSocketRequest { get; }
    //bool KeepAlive { get; }
    NameValueCollection QueryString { get; }
    //string RawUrl { get; }
    string UserAgent { get; }
    string UserHostAddress { get; }
    string UserHostName { get; }
    Uri UrlReferrer { get; }
    Uri Url { get; }
    Version ProtocolVersion { get; }
    //X509Certificate2 GetClientCertificate();
    //IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state);
    //Task<X509Certificate2> GetClientCertificateAsync();
    //int ClientCertificateError { get; }
    IPEndPoint RemoteEndPoint { get; }

    NameValueCollection Headers { get; }
    string HttpMethod { get; }
    Stream InputStream { get; }
    bool IsSecureConnection  { get; }
    bool HasEntityBody { get; }
}