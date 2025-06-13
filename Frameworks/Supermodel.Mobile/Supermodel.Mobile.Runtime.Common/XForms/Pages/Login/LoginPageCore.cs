using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.DataContext.WebApi;
using Supermodel.Mobile.Runtime.Common.Exceptions;
using Supermodel.Mobile.Runtime.Common.XForms.Views;
using Xamarin.Forms;   
using Supermodel.Mobile.Runtime.Common.XForms.App;
using System.IO;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public abstract class LoginPageCore<TLoginViewModel, TLoginView> : ContentPage, IHaveActivityIndicator
    where TLoginViewModel: ILoginViewModel, new()
    where TLoginView : LoginViewBase<TLoginViewModel>, new()
{
    #region Contructors
    protected LoginPageCore()
    {
        Content = LoginView = new ViewWithActivityIndicator<TLoginView>(new TLoginView());
        AutoLoginIfConnectionLost = false;
        LoginView.ContentView.SetUpSignInClickedHandler(SignInClicked);
    }
    #endregion

    #region Methods
    public abstract Task<LoginResult> TryLoginAsync();
    public abstract IAuthHeaderGenerator GetAuthHeaderGenerator(TLoginViewModel loginViewModel);
    public IAuthHeaderGenerator GetBlankAuthHeaderGenerator()
    {
        var authHeader = GetAuthHeaderGenerator(new TLoginViewModel());
        authHeader.Clear();
        return authHeader;
    }
    public async Task AutoLoginIfPossibleAsync()
    {
        var authHeaderGenerator = GetBlankAuthHeaderGenerator();
        if (authHeaderGenerator.LoadFromAppProperties())
        {
            bool connectionLost;
            do
            {
                connectionLost = false;
                try
                {
                    using(var activityIndicator = new ActivityIndicatorFor(LoginView, "Logging you in..."))
                    {
                        FormsApplication.GetRunningApp().AuthHeaderGenerator = authHeaderGenerator;
                        var loginResult = await TryLoginAsync();
                        if (loginResult.LoginSuccessful)
                        {
                            if (!string.IsNullOrEmpty(loginResult.UserLabel)) activityIndicator.Element.Message = "Welcome, " + loginResult.UserLabel + "!";

                            FormsApplication.GetRunningApp().AuthHeaderGenerator.UserId = loginResult.UserId; 
                            FormsApplication.GetRunningApp().AuthHeaderGenerator.UserLabel = loginResult.UserLabel;

                            await Task.Delay(800); //short delay so that the message can be read
                            if (await DoLoginAsync(true, false)) await FormsApplication.GetRunningApp().AuthHeaderGenerator.SaveToAppPropertiesAsync();
                        }
                        else
                        {
                            await authHeaderGenerator.ClearAndSaveToPropertiesAsync();
                            FormsApplication.GetRunningApp().AuthHeaderGenerator = null;
                        }
                    }
                }
                catch (SupermodelWebApiException ex1)
                {
                    connectionLost = true;
                    var result = await DisplayAlert("Server Error", ex1.ContentJsonMessage, "Cancel", "Try again");
                    if (result) return;
                }
                catch (Exception netEx) when (netEx is HttpRequestException || netEx is IOException || netEx is WebException)
                {
                    if (AutoLoginIfConnectionLost)
                    {
                        await DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Work Offline");
                        using (new ActivityIndicatorFor(LoginView))
                        {
                            await DoLoginAsync(true, false);
                        }
                        return;
                    }
                        
                    var result = await DisplayAlert("Connection Lost", "Connection to the cloud cannot be established.", "Cancel", "Try again");
                    if (result) return;
                }
                catch (Exception ex3)
                {
                    connectionLost = true;
                    var result = await DisplayAlert("Unexpected Error", ex3.Message, "Cancel", "Try again");
                    if (result) return;
                }
            }
            while(connectionLost);            
        }
    }
    #endregion

    #region Overrides
    protected virtual void OnDisappearingBase()
    {
        // ReSharper disable once RedundantBaseQualifier
        base.OnDisappearing();
    }
    protected virtual void OnAppearingBase()
    {
        // ReSharper disable once RedundantBaseQualifier
        base.OnAppearing();
    }
    protected override async void OnAppearing()
    {
        PageActive = true;

        if (_trueTitle != null)
        {
            Title = _trueTitle;
            _trueTitle = null;
        }
            
        //If Sign Out is clicked
        if (_loggedIn)
        {
            var answer = await DisplayAlert("Alert", "Are you sure you want to sign out?", "Yes", "No");

            if (answer)
            {
                if (!await OnConfirmedLogOutAsync()) await DoLoginAsync(true, true);
            }
            else
            {
                await DoLoginAsync(true, true);
            }
        }
        OnAppearingBase();
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        PageActive = false;
    }
    protected virtual async Task<bool> OnConfirmedLogOutAsync()
    {
        await FormsApplication.GetRunningApp().AuthHeaderGenerator.ClearAndSaveToPropertiesAsync();
        FormsApplication.GetRunningApp().AuthHeaderGenerator = null;
        _loggedIn = false;
        return true;
    }
    protected virtual async Task<bool> DoLoginAsync(bool autoLogin, bool isJumpBack)
    {
        _trueTitle = Title;
        Title = "Sign Out";

        var result = await OnSuccessfulLoginAsync(autoLogin, isJumpBack);

        if (result) _loggedIn = true;
        else Title = _trueTitle;

        return result;
    }
    public abstract Task<bool> OnSuccessfulLoginAsync(bool autoLogin, bool isJumpBack);
    #endregion

    #region IHaveActivityIndicator implementation
    public async Task WaitForPageToBecomeActiveAsync()
    {
        while(!PageActive) await Task.Delay(25);
    }
    public bool ActivityIndicatorOn
    {
        get => LoginView.ActivityIndicatorOn;
        set => LoginView.ActivityIndicatorOn = value;
    }
    public string Message
    {
        get => LoginView.Message;
        set => LoginView.Message = value;
    }
    #endregion

    #region Event Handlers
    public virtual async void SignInClicked(object sender, EventArgs args)
    {
        var loginViewModel = LoginView.ContentView.ViewModel;
        var validationError = loginViewModel.GetValidationError();
        if (validationError != null)
        {
            FormsApplication.GetRunningApp().AuthHeaderGenerator = null;
            await DisplayAlert("Login", validationError, "Ok");
            return;
        }

        bool connectionLost;
        do
        {
            connectionLost = false;
            try
            {
                using(var activityIndicator = new ActivityIndicatorFor(LoginView, "Logging you in..."))
                {
                    FormsApplication.GetRunningApp().AuthHeaderGenerator = GetAuthHeaderGenerator(loginViewModel);
                    var loginResult = await TryLoginAsync();
                    if (loginResult.LoginSuccessful)
                    {
                        if (!string.IsNullOrEmpty(loginResult.UserLabel)) activityIndicator.Element.Message = "Welcome, " + loginResult.UserLabel + "!";

                        FormsApplication.GetRunningApp().AuthHeaderGenerator.UserId = loginResult.UserId; 
                        FormsApplication.GetRunningApp().AuthHeaderGenerator.UserLabel = loginResult.UserLabel;

                        await Task.Delay(800); //short delay so that the message can be read
                        if (await DoLoginAsync(false, false)) await FormsApplication.GetRunningApp().AuthHeaderGenerator.SaveToAppPropertiesAsync();
                    }
                    else
                    {
                        FormsApplication.GetRunningApp().AuthHeaderGenerator = null;
                        await DisplayAlert("Unable to sign in", "Username and password combination provided is invalid. Please try again.", "Ok");
                    }
                }
            }
            catch (SupermodelWebApiException ex1)
            {
                connectionLost = true;
                var result = await DisplayAlert("Server Error", ex1.ContentJsonMessage, "Cancel", "Try again");
                if (result) return;
            }
            catch (Exception netEx) when (netEx is HttpRequestException || netEx is IOException || netEx is WebException)
            {
                connectionLost = true;
                var result = await DisplayAlert("Server Error", "Connection to the cloud cannot be established.", "Cancel", "Try again");
                if (result) return;
            }
            catch (Exception ex3)
            {
                connectionLost = true;
                var result = await DisplayAlert("Unexpected Error", ex3.Message, "Cancel", "Try again");
                if (result) return;
            }
        }
        while(connectionLost);
    }
    #endregion

    #region Properties
    public ViewWithActivityIndicator<TLoginView> LoginView { get; set; }
    public bool AutoLoginIfConnectionLost { get; set; }
        
    // ReSharper disable InconsistentNaming
    protected bool _loggedIn;
    protected string _trueTitle = "Sign In";
    // ReSharper restore InconsistentNaming

    protected bool PageActive { get; set; }
    #endregion        
}