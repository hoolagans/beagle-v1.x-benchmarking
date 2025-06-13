#nullable disable

using System;
using System.IO;
using System.Net;

namespace WebMonk.Context.WMHttpListenerObjects;

public interface IHttpListenerResponse : IDisposable
{
    WebHeaderCollection Headers { get; set; }
    //Encoding ContentEncoding { get; set; }
    string ContentType { get; set; }
    //bool SendChunked { get; set; }
    //long ContentLength64 { get; set; }
    CookieCollection Cookies { get; set; }
    //bool KeepAlive { get; set; }
    Stream OutputStream { get; }
    string RedirectLocation { get; set; }
    string StatusDescription { get; set; }
    void AddHeader(string name, string value);
    void AppendHeader(string name, string value);
    void AppendCookie(Cookie cookie);
    //void Redirect(string url);
    void SetCookie(Cookie cookie);

    int StatusCode { get; set; }
    //void CopyFrom(HttpListenerResponse templateResponse);
    Version ProtocolVersion { get; set; }
    //void Abort();
    void Close();
    //void Close(byte[] responseEntity, bool willBlock);

}