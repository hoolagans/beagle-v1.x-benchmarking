using System.Threading.Tasks;
using WebMonk.Extensions;
using WebMonk.HttpRequestHandlers.Controllers;
using WebMonk.Results;

namespace HTML2RazorSharpWM.Mvc.MainPage;

public class MainMvcController : MvcController
{
    #region Methods
    public Task<ActionResult> GetIndexAsync()
    {
        return Task.FromResult<ActionResult>(new MainMvcView().RenderIndex().ToHtmlResult());
        //return Task.FromResult<ActionResult>(new ProspectSignUpMvcView().RenderProspectSignUp(new ProspectSignUpMvcModel()).ToHtmlResult());

    }
    #endregion
}