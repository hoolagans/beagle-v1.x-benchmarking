#nullable disable

using WebMonk.Context.WMHttpListenerObjects;

namespace Supermodel.Presentation.WebMonk.Batch;

public class BatchHttpListenerContext
    (IHttpListenerContext rootContext, string httpRequestRawStr) : IHttpListenerContext
{
    #region Properties
    public IHttpListenerRequest Request { get; set; } = new BatchHttpListenerRequest(httpRequestRawStr, rootContext.Request);
    public IHttpListenerResponse Response { get; set; } = new BatchHttpListenerResponse(rootContext.Response);

    #endregion
}