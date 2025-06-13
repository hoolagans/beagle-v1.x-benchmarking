using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebMonk.Context;

namespace WebMonk.Results;

public class TextResult : ActionResult
{
    #region Constructors
    public TextResult(HttpStatusCode statusCode, string body, string contentType)
    {
        StatusCode = statusCode;
        Body = body;
        ContentType = contentType;
    }
    #endregion
        
    #region Overrides
    public override async Task ExecuteResultAsync()
    {
        var response = HttpContext.Current.HttpListenerContext.Response;

        response.ContentType = ContentType;
        response.StatusCode = (int)StatusCode;
        var messageBytes = Encoding.Default.GetBytes(Body);
        await response.OutputStream.WriteAsync(messageBytes, 0, messageBytes.Length).ConfigureAwait(false);
    }
    #endregion

    #region Properties
    public HttpStatusCode StatusCode { get; }
    public string Body { get; }
    public string ContentType { get; }
    #endregion
}