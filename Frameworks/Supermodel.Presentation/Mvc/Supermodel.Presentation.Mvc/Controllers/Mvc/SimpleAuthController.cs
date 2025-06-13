using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermodel.Presentation.Mvc.Extensions.Gateway;
using Supermodel.Presentation.Mvc.Models.Mvc;

namespace Supermodel.Presentation.Mvc.Controllers.Mvc;

[AllowAnonymous]
public abstract class SimpleAuthController<TLoginMvcModel> : Controller where TLoginMvcModel : class, ILoginMvcModel, new()
{
    #region Action Methods
    [HttpGet]
    public virtual ActionResult LogIn()
    {
        return View(new TLoginMvcModel());
    }

    [HttpPost]
    public virtual async Task<IActionResult> LogIn(TLoginMvcModel login, string? returnUrl)
    {
        await HttpContext.SignOutAsync();

        var claims = await AuthenticateAndGetClaimsAsync(login.UsernameStr, login.PasswordStr);
        if (claims.All(x => x.Type != ClaimTypes.NameIdentifier))
        {
            TempData.Super().NextPageModalMessage = "Username and password combination is incorrect!";
            login.PasswordStr = "";
            return View(login);
        }
        await DoSignInAsync(claims);

        return string.IsNullOrEmpty(returnUrl) ? RedirectToHomeScreen() : RedirectToLocal(returnUrl);
    }

    public virtual async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        // ReSharper disable once Mvc.ActionNotResolved
        return RedirectToAction("LogIn");
    }
    #endregion

    #region Methods for Overrides
    protected abstract Task<List<Claim>> AuthenticateAndGetClaimsAsync(string username, string password);
    protected abstract IActionResult RedirectToHomeScreen();
    protected virtual Task DoSignInAsync(List<Claim> claims)
    {
        var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new ClaimsPrincipal(identity);
        return HttpContext.SignInAsync(scheme, principal);
    }
    #endregion

    #region Protected Helpers
    protected virtual IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        else return RedirectToHomeScreen();
    }
    #endregion
}