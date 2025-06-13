using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Supermodel.DataAnnotations.Validations;
using WebMonk.Context.WMHttpListenerObjects;
using WebMonk.Session;

namespace WebMonk.Context;

public class HttpContext
{
    #region Constructors
    internal HttpContext(WebServer webServer, 
        IHttpListenerContext httpListenerContext, 
        IRouteManager routeManager, 
        IPrefixManager prefixManager, 
        IValueProviderManager valueProviderManager,
        StaticModelBinderManagerBase staticModelBinderManager, 
        SessionState sessionState)
    {
        WebServer = webServer;
        HttpListenerContext = httpListenerContext;

        RouteManager = routeManager;
        PrefixManager = prefixManager;
        ValueProviderManager = valueProviderManager;
        StaticModelBinderManager = staticModelBinderManager;
        SessionState = sessionState;

        ValidationResultList = new ValidationResultList();
    }
    #endregion
        
    #region Methods
    public void AuthenticateSessionWithClaims(IEnumerable<Claim> claims)
    {
        SessionState.AuthenticateSessionWithClaims(claims);
    }
    public void UnAuthenticateSession()
    {
        SessionState.UnAuthenticateSession();
    }

    public static HttpContext Current => HttpContextScopeCore.CurrentHttpContext;
    public static bool ExistsCurrent => HttpContextScopeCore.StackCount > 0;
    #endregion

    #region Properties
    public WebServer WebServer { get; }
    public IHttpListenerContext HttpListenerContext { get; }

    public string SessionId => SessionState.SessionId;
    public SessionDictionary Session => SessionState.Session;
    public TempDataDictionary TempData => SessionState.TempData;
    public ImmutableList<Claim> Claims => SessionState.Claims;
    internal SessionState SessionState { get; }

    public IRouteManager RouteManager { get; }
    public IPrefixManager PrefixManager { get; }
    public IValueProviderManager ValueProviderManager { get; }
    public StaticModelBinderManagerBase StaticModelBinderManager { get; }

    public bool BlockDangerousValueProviderValues { get; set; } = true;

    public ValidationResultList ValidationResultList { get; }

    public ConcurrentDictionary<string, object?> CustomValues { get; } = new();
    #endregion
}