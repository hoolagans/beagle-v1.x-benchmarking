using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.DataContext.WebApi;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.Mobile.Runtime.Common.UnitOfWork;
using Supermodel.Mobile.Runtime.Common.XForms.App;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public abstract class LoginPageBase<TLoginViewModel, TLoginView, TLoginValidationModel, TWebApiDataContext> : LoginPageCore<TLoginViewModel, TLoginView>
    where TLoginViewModel: ILoginViewModel, new()
    where TLoginView : LoginViewBase<TLoginViewModel>, new()
    where TLoginValidationModel : class, IModel
    where TWebApiDataContext : WebApiDataContext, new()
{
    #region Overrides
    public override async Task<LoginResult> TryLoginAsync()
    {
        await using(FormsApplication.GetRunningApp().NewUnitOfWork<TWebApiDataContext>(ReadOnly.Yes))
        {
            return await UnitOfWorkContext.ValidateLoginAsync<TLoginValidationModel>();
        }
    }
    #endregion
}