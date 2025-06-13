using System;
using Xamarin.Forms;   

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public class UsernameAndPasswordLoginView : LoginViewBase<UsernameAndPasswordLoginViewModel>
{
    #region Constructors
    public UsernameAndPasswordLoginView()
    {
        VerticalOptions = LayoutOptions.FillAndExpand;
        HorizontalOptions = LayoutOptions.FillAndExpand;
        Padding = new Thickness(15, 15);

        BindingContext = ViewModel = new UsernameAndPasswordLoginViewModel();

        Username = new Entry { Placeholder = "Username", Keyboard = Keyboard.Email };
        Username.SetBinding(Entry.TextProperty, "Username");
        Children.Add(Username);

        Password = new Entry { Placeholder = "Password", IsPassword = true };
        Password.SetBinding(Entry.TextProperty, "Password");
        Children.Add(Password);

        SignInButton = new Button { Text = "Sign In" };
            
        Children.Add(SignInButton);
    }
    #endregion

    #region Methods
    public virtual void SetUpLoginImage(Image image)
    {
        Children.Insert(0, image);
    }
    public override void SetUpSignInClickedHandler(EventHandler handler)
    {
        SignInButton.Clicked += handler;
    }
    #endregion

    #region Properties
    public Entry Username { get; set; }
    public Entry Password { get; set; }
    public Button SignInButton { get; set; }
    #endregion
}