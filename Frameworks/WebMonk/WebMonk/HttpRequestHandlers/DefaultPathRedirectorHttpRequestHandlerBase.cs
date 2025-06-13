namespace WebMonk.HttpRequestHandlers;

public abstract class DefaultPathRedirectorHttpRequestHandlerBase : PathRedirectorHttpRequestHandlerBase
{
    #region Constructors
    protected DefaultPathRedirectorHttpRequestHandlerBase(string redirectTo) : base("/", redirectTo) { }
    #endregion
}