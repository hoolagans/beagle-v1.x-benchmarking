using System;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public abstract class LoginViewBase<TLoginViewModel> : StackLayout where TLoginViewModel: ILoginViewModel, new()
{
    #region Methods
    public abstract void SetUpSignInClickedHandler(EventHandler handler);
    #endregion

    #region Properties
    public TLoginViewModel ViewModel { get; set; }
    #endregion
}