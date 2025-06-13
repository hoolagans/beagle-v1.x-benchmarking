using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Extensions.Gateway;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views;
using WebMonk.Context;
using WebMonk.Extensions;
using WebMonk.HttpRequestHandlers.Controllers;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Mvc;

public abstract class SimpleAuthMvcController<TLoginMvcModel, TAuthMvcView> : MvcController 
    where TLoginMvcModel : class, ILoginMvcModel, new()
    where TAuthMvcView: SimpleAuthViewBase<TLoginMvcModel>, new()
{
    #region Action Methods
    public virtual ActionResult GetLogIn()
    {
        return new TAuthMvcView().RenderLogin(new TLoginMvcModel()).ToHtmlResult();
    }

    public virtual async Task<ActionResult> PostLogInAsync(TLoginMvcModel login, string? returnUrl = null)
    {
        var claims = await AuthenticateAndGetClaimsAsync(login.UsernameStr, login.PasswordStr).ConfigureAwait(false);
        if (claims.All(x => x.Type != ClaimTypes.NameIdentifier))
        {
            TempData.Super().NextPageModalMessage = "Username and password combination is incorrect!";
            login.PasswordStr = "";
            return new TAuthMvcView().RenderLogin(login).ToHtmlResult();
        }
        HttpContext.Current.AuthenticateSessionWithClaims(claims);

        return string.IsNullOrEmpty(returnUrl) ? RedirectToHomeScreen() : RedirectToLocal(returnUrl);
    }

    public virtual ActionResult GetLogOut()
    {
        HttpContext.Current.UnAuthenticateSession();
        return RedirectToAction("LogIn");
    }
    #endregion

    #region Methods for Overrides
    protected abstract Task<List<Claim>> AuthenticateAndGetClaimsAsync(string username, string password);
    protected abstract LocalRedirectResult RedirectToHomeScreen();
    #endregion
}