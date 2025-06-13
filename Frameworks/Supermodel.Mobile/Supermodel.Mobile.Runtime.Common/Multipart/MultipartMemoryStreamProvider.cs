
namespace Supermodel.Mobile.Runtime.Common.Multipart;

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

public class MultipartMemoryStreamProvider : MultipartStreamProvider
{
    public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
    {
        if (parent == null) throw new ArgumentNullException("parent");
        if (headers == null) throw new ArgumentNullException("headers");
        return new MemoryStream();
    }
}