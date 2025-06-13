using System;
using Supermodel.DataAnnotations.Exceptions;

namespace WebMonk.Context;

public class HttpContextScope : IDisposable
{
    #region Constructors
    public HttpContextScope(HttpContext httpContext)
    {
        Context = httpContext;
        HttpContextScopeCore.PushHttpContext(Context);
    }
    #endregion

    #region IAsyncDisposable implementation
    public void Dispose()
    {
        var context = HttpContextScopeCore.PopHttpContext();
        if (context != Context) throw new SupermodelException("HttpContextScope: POP on Dispose popped mismatched HttpContext.");
    }
    #endregion

    #region Properties
    public HttpContext Context { get; }
    #endregion
}