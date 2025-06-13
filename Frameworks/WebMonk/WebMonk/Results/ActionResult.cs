using WebMonk.HttpRequestHandlers;

namespace WebMonk.Results;

public abstract class ActionResult : IHttpRequestHandler.HttpRequestHandlerResult
{
    protected ActionResult() : base(true) { }
}