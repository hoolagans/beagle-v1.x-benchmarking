#nullable disable

using System;
using System.Buffers;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Supermodel.DataAnnotations.Exceptions;
using WebMonk.Context.WMHttpListenerObjects;
using WebMonk.Multipart;

namespace Supermodel.Presentation.WebMonk.Batch;
//https://stackoverflow.com/questions/318506/converting-raw-http-request-into-httpwebrequest-object

public class BatchHttpListenerRequest : IHttpListenerRequest
{
    #region Constructors
    public BatchHttpListenerRequest(string httpRequestRawStr, IHttpListenerRequest rootRequest)
    {
        var requestRaw = Encoding.UTF8.GetBytes(httpRequestRawStr);
        var buffer = new ReadOnlySequence<byte>(requestRaw).ToArray();
            
        var parsedHttpRequest = new HttpUnsortedRequest();
        var parser = new HttpRequestHeaderParser(parsedHttpRequest);
            
        ParserState parseStatus;
        var headerConsumed = 0;
        try
        {
            parseStatus = parser.ParseBuffer(buffer, buffer.Length, ref headerConsumed);
        }
        catch (Exception)
        {
            parseStatus = ParserState.Invalid;
        }

        if (parseStatus != ParserState.Done) throw new SupermodelException($"Invalid HttpRequest in batch: {parseStatus}");

        //these properties we steal from the root request
        Cookies = rootRequest.Cookies;
        ContentEncoding = rootRequest.ContentEncoding;
        ProtocolVersion = rootRequest.ProtocolVersion;
        UserHostAddress = rootRequest.UserHostAddress;
        UrlReferrer = rootRequest.UrlReferrer;
        IsSecureConnection = rootRequest.IsSecureConnection;
        UserAgent = rootRequest.UserAgent;
            
        //This is the stuff we parsed
        HttpMethod = parsedHttpRequest.Method.ToString().ToUpper();
            
        var scheme = IsSecureConnection ? "https://" : "http://";
        var fullPath = $"{scheme}{UserHostAddress}{parsedHttpRequest.RequestUri}";
        Url = new Uri(fullPath);            
        QueryString = HttpUtility.ParseQueryString(Url.Query);

        foreach (var header in parsedHttpRequest.HttpHeaders)
        {
            foreach (var value in header.Value)
            {
                Headers.Add(header.Key, value);
                if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase)) ContentType = value;
            }
        }

        //Message body is what we have remaining
        buffer = buffer[headerConsumed..];
        HasEntityBody = buffer.Length > 0;
        InputStream = new MemoryStream(buffer);

        RemoteEndPoint = rootRequest.RemoteEndPoint;
    }
    #endregion

    #region IHttpListenerRequest implementation
    public CookieCollection Cookies { get; set; }
    public Encoding ContentEncoding { get; set; }
    public string ContentType { get; set; }
    public NameValueCollection QueryString { get; set; }
    public string UserAgent { get; set; }
    public string UserHostAddress { get; set; }
    public string UserHostName { get; set; }
    public Uri UrlReferrer { get; set; }
    public Uri Url { get; set; }
    public Version ProtocolVersion { get; set; }
    public NameValueCollection Headers { get; set; } = new();
    public string HttpMethod { get; set; }
    public Stream InputStream { get; set; }
    public bool IsSecureConnection { get; set; }
    public bool HasEntityBody { get; set; }
    public IPEndPoint RemoteEndPoint { get; set; }
    #endregion
}