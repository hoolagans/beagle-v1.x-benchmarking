using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebMonk.Context;
using WebMonk.Context.WMHttpListenerObjects;
using WebMonk.Exceptions;

namespace WebMonk.Session;

internal class SessionState
{
    #region Constructors & Factory Methods
    public static SessionState GetOrCreate(string sessionId)
    {
        if (!SessionStatesDict.TryGetValue(sessionId, out var sessionState)) 
        {
            sessionState = new SessionState(sessionId);
            sessionState.IsNew = true;
        }
        else 
        {
            sessionState.TempData.MoveFutureIntoCurrent(sessionId);
            sessionState.IsNew = false;
        }
            
        return sessionState;
    }
    protected SessionState(string sessionId)
    {
        SessionId = sessionId;
    }
    #endregion

    #region Methods
    public void SaveSessionState()
    {
        LastLoaded = DateTime.Now;
        SessionStatesDict.TryAdd(SessionId, this);
        if (IsNew)
        {
            var cookie = new Cookie(SessionCookieName, SessionId) { HttpOnly = true, Path = "/" };
            HttpContext.Current.HttpListenerContext.Response.SetCookie(cookie);
        }
    }
    public void AuthenticateSessionWithClaims(IEnumerable<Claim> claims)
    {
        var list = claims.ToImmutableList();
        if (list.Count(x => x.Type == ClaimTypes.NameIdentifier) != 1) throw new WebMonkException("Cannot AuthenticateSessionWithClaims: exactly one of the claims must be of type ClaimTypes.NameIdentifier");

        Claims = list;
            
        SessionStatesDict.TryRemove(SessionId, out _);

        //when we authenticate, we set a new session cookie
        SessionId = GenerateNewSessionId();
        IsNew = true;
        LastLoaded = DateTime.Now;
    }
    public void UnAuthenticateSession()
    {
        Claims = ImmutableList<Claim>.Empty;
    }
    public static async Task RemoveExpiredTasksServiceAsync(int sessionTimeout, CancellationToken cancellationToken)
    {
        while(!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(5 * 60_000, cancellationToken).ConfigureAwait(false); //run every 5 minutes
            var now = DateTime.Now;
            foreach (var key in SessionStatesDict.Keys.ToArray())
            {
                if (SessionStatesDict[key].LastLoaded.AddMinutes(sessionTimeout) < now) SessionStatesDict.TryRemove(key, out _);
            }
        }
    }

    public static string ManageSessionCookie(IHttpListenerContext httpListenerContext)
    {
        string sessionId;
            
        // ReSharper disable once SuspiciousTypeConversion.Global
        var cookies = (IEnumerable<Cookie>)httpListenerContext.Request.Cookies;
        var sessionCookie = cookies.FirstOrDefault(x => x.Name == SessionCookieName);

        if (sessionCookie != null) sessionId = sessionCookie.Value;
        else sessionId = GenerateNewSessionId();

        return sessionId;
    }
    public static string GenerateNewSessionId()
    {
        var sessionIdPt1 = Convert.ToBase64String(Hasher.ComputeHash(Encoding.Unicode.GetBytes($"{Guid.NewGuid()}{Guid.NewGuid()}")))[..86];
        var sessionIdPt2 = Convert.ToBase64String(Hasher.ComputeHash(Encoding.Unicode.GetBytes($"{Guid.NewGuid()}{Guid.NewGuid()}")))[..86];
        var sessionId = sessionIdPt1 + sessionIdPt2;

        var chrIndex = Rnd.Next(0, 26);
        var chr = (char)('a' + chrIndex);
        var index = Rnd.Next(0, sessionId.Length - 2);
        sessionId = sessionId.Insert(index, chr.ToString());
        sessionId = $"{index:x2}{chrIndex:x2}{sessionId}";
        return sessionId;
    }
    #endregion

    #region Properties
    public bool IsBlank => Claims.IsEmpty && _session == null && _tempData == null;
    public bool IsNew { get; protected set; } = true;

    public string SessionId { get; protected set; }
    public DateTime LastLoaded { get; protected set; }

    public ImmutableList<Claim> Claims { get; set; } = ImmutableList<Claim>.Empty;

    public SessionDictionary Session 
    { 
        get
        {
            if (_session == null) _session = new SessionDictionary();
            return _session;
        }
    }
    protected SessionDictionary? _session;

    public TempDataDictionary TempData 
    { 
        get
        {
            if (_tempData == null) _tempData = new TempDataDictionary();
            return _tempData;
        }
    }
    protected TempDataDictionary? _tempData;
        
    public static ConcurrentDictionary<string, SessionState> SessionStatesDict { get; } = new();

    //Random is not thread-safe, so we create an instance for every thread
    public static Random Rnd => _rnd ??= new Random(Guid.NewGuid().GetHashCode());
    [ThreadStatic] private static Random? _rnd;

    //SHA512CryptoServiceProvider is not thread-safe, so we create an instance for every thread
    public static SHA512CryptoServiceProvider Hasher => _hasher ??= new SHA512CryptoServiceProvider();
    [ThreadStatic] private static SHA512CryptoServiceProvider? _hasher;
    #endregion

    #region Constants
    public const string SessionCookieName = ".WMSession";
    #endregion
}