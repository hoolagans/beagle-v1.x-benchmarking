using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Encryptor;
using WebMonk.Context;
using WebMonk.Filters.Base;

namespace Supermodel.Presentation.WebMonk.Auth;

public abstract class SupermodelAuthenticateAttributeBase : ActionFilterAttribute
{
    #region overrides
    public override async Task<ActionFilterResult> BeforeActionAsync(ActionFilterContext filterContext)
    {
        var claims = await HandleAuthenticateAsync().ConfigureAwait(false);
        if (claims.Count > 0) HttpContext.Current.AuthenticateSessionWithClaims(claims);
        return ActionFilterResult.Proceed;
    }
    protected virtual async Task<List<Claim>> HandleAuthenticateAsync()
    {
        var headerValues = HttpContext.Current.HttpListenerContext.Request.Headers.GetValues(AuthHeaderName);
        if (headerValues == null) return new List<Claim>();
            
        var authHeader = headerValues.SingleOrDefault();
        if (string.IsNullOrEmpty(authHeader)) return new List<Claim>();

        List<Claim> claims;
        if (authHeader.StartsWith("Basic "))
        {
            HttpAuthAgent.ReadBasicAuthToken(authHeader, out var username, out var password);
            claims = await AuthenticateBasicAndGetClaimsAsync(username, password).ConfigureAwait(false);
        }
        else if (authHeader.StartsWith("SMCustomEncrypted "))
        {
            HttpAuthAgent.ReadSMCustomEncryptedAuthToken(EncryptionKey, authHeader, out var args);
            claims = await AuthenticateEncryptedAndGetClaimsAsync(args).ConfigureAwait(false);
        }
        else
        {
            throw new SupermodelException("Invalid authorization scheme. 'Basic' or 'SMCustomEncrypted' expected");
        }

        return claims;
    }
    public override int Priority => -200;
    #endregion

    #region Abstract Memebers
    protected abstract Task<List<Claim>> AuthenticateBasicAndGetClaimsAsync(string username, string password);
    protected abstract Task<List<Claim>> AuthenticateEncryptedAndGetClaimsAsync(string[] args);
    protected abstract byte[] EncryptionKey { get; }
    #endregion

    #region Virtual Methods
    protected virtual string AuthHeaderName => "Authorization";
    #endregion

    #region Consts
    public const string AuthenticationScheme = "WebApi";
    #endregion
}