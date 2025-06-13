using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Supermodel.Presentation.Mvc.Context;
using Supermodel.Presentation.Mvc.Models.Api;

namespace Supermodel.Presentation.Mvc.Controllers.Api;

[ApiController, Route("[controller]", Order = 1), Route("[controller]/[action]", Order = 2)]
public abstract class ApiControllerBase : ControllerBase
{
    #region Action Methods
    [HttpGet, Authorize]
    public virtual async Task<IActionResult> ValidateLogin()
    {
        //Authorize attribute is the key here
        return StatusCode((int)HttpStatusCode.OK, await GetValidateLoginResponseAsync());
    }
    #endregion

    #region Protected Helpers
    protected virtual Task<ValidateLoginResponseApiModel> GetValidateLoginResponseAsync()
    {
        return Task.FromResult(new ValidateLoginResponseApiModel { UserId = RequestHttpContext.CurrentUserId, UserLabel = RequestHttpContext.CurrentUserLabel});
    }
    #endregion
}