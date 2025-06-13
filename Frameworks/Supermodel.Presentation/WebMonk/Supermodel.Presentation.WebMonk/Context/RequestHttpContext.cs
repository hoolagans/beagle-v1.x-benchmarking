using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebMonk.Context;

namespace Supermodel.Presentation.WebMonk.Context;

public static class RequestHttpContext
{
    #region Methods & Method-like Properties
    public static long? CurrentUserId
    {
        get
        {
            var userNameIdentifierClaim = HttpContext.Current.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userNameIdentifierClaim == null) return null;
            return long.Parse(userNameIdentifierClaim.Value);
        }
    }
    public static string? CurrentUserLabel
    {
        get
        {
            var userNameClaim = HttpContext.Current.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
            if (userNameClaim == null) return null;
            return userNameClaim.Value;
        }
    }

    public static IEnumerable<Claim> CurrentUserClaims => HttpContext.Current.Claims;
    public static bool IsCurrentUserInRole(string role) => CurrentUserClaims.Any(x => x.Type == ClaimTypes.Role && x.Value == role);
    #endregion
}