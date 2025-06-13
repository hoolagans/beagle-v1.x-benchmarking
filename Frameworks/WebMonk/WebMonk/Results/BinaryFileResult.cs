using System.Net;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Extensions;
using WebMonk.Context;

namespace WebMonk.Results;

public class BinaryFileResult : ActionResult
{
    #region Constructors
    public BinaryFileResult(byte[] body, string fileName, string contentType = "application/octet-stream", bool suggestOpenInline = false)
    {
        Body = body;
        FileName = fileName;
        ContentType = contentType;
        SuggestOpenInline = suggestOpenInline;
    }
    #endregion
        
    #region Overrides
    public override async Task ExecuteResultAsync()
    {
        var response = HttpContext.Current.HttpListenerContext.Response;

        response.ContentType = ContentType;
        response.StatusCode = (int)StatusCode;
        
        if (SuggestOpenInline) response.AddHeader("Content-Disposition", $"inline; filename=\"{FileName.HttpHeaderEncode()}\"");
        else response.AddHeader("Content-Disposition", $"attachment; filename=\"{FileName.HttpHeaderEncode()}\"");

        await response.OutputStream.WriteAsync(Body, 0, Body.Length).ConfigureAwait(false);
    }
    #endregion

    #region Properties
    public byte[] Body { get; }
    public string FileName { get; }
    public string ContentType { get; }
    public bool SuggestOpenInline { get; }
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    #endregion
}