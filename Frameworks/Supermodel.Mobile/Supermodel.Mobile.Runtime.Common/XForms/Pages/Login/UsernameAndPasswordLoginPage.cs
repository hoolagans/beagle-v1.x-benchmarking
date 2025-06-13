using Supermodel.Mobile.Runtime.Common.DataContext.WebApi;
using Supermodel.Mobile.Runtime.Common.Models;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public abstract class UsernameAndPasswordLoginPage<TLoginValidationModel, TWebApiDataContext> : LoginPageBase<UsernameAndPasswordLoginViewModel, UsernameAndPasswordLoginView, TLoginValidationModel, TWebApiDataContext>
    where TLoginValidationModel : class, IModel
    where TWebApiDataContext : WebApiDataContext, new()
{}