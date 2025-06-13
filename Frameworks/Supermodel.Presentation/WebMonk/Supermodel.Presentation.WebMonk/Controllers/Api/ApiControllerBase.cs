using System.Threading.Tasks;
using Supermodel.Presentation.WebMonk.Context;
using Supermodel.Presentation.WebMonk.Models.Api;
using WebMonk.Filters;
using WebMonk.HttpRequestHandlers.Controllers;
using WebMonk.Results;

namespace Supermodel.Presentation.WebMonk.Controllers.Api;

public abstract class ApiControllerBase : ApiController
{
    #region Action Methods
    [Authorize]
    public virtual async Task<ActionResult> GetValidateLoginAsync()
    {
        //Authorize attribute is the key here
        return new JsonApiResult(await GetValidateLoginResponseAsync().ConfigureAwait(false));
    }
    #endregion

    #region Protected Helpers
    protected virtual Task<ValidateLoginResponseApiModel> GetValidateLoginResponseAsync()
    {
        return Task.FromResult(new ValidateLoginResponseApiModel { UserId = RequestHttpContext.CurrentUserId, UserLabel = RequestHttpContext.CurrentUserLabel});
    }
    #endregion
}