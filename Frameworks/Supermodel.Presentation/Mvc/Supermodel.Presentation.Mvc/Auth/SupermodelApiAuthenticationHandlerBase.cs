using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Encryptor;

namespace Supermodel.Presentation.Mvc.Auth;

public abstract class SupermodelApiAuthenticationHandlerBase : AuthenticationHandler<AuthenticationSchemeOptions>
{
    #region Constructors
    protected SupermodelApiAuthenticationHandlerBase(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder){}
    #endregion

    #region Overrides
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers[AuthHeaderName].SingleOrDefault();
        if (string.IsNullOrEmpty(authHeader)) return AuthenticateResult.NoResult();

        List<Claim> claims;
        // ReSharper disable once PossibleNullReferenceException
        if (authHeader.StartsWith("Basic "))
        {
            HttpAuthAgent.ReadBasicAuthToken(authHeader, out var username, out var password);
            claims = await AuthenticateBasicAndGetClaimsAsync(username, password);
        }
        else if (authHeader.StartsWith("SMCustomEncrypted "))
        {
            HttpAuthAgent.ReadSMCustomEncryptedAuthToken(EncryptionKey, authHeader, out var args);
            claims = await AuthenticateEncryptedAndGetClaimsAsync(args);
        }
        else
        {
            throw new SupermodelException("Invalid authorization scheme. 'Basic' or 'SMCustomEncrypted' expected");
        }

        if (claims.All(x => x.Type != ClaimTypes.NameIdentifier)) return AuthenticateResult.Fail("Invalid Username or Password");

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
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