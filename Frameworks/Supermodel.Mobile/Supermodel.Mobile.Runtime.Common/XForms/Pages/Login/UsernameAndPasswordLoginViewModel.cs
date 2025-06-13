using System.ComponentModel;
using System.Runtime.CompilerServices;
using Supermodel.Mobile.Runtime.Common.XForms.App;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public class UsernameAndPasswordLoginViewModel : ILoginViewModel	
{
    #region InotifyPropertyChanged implementation
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion

    #region Overrides
    public virtual IAuthHeaderGenerator GetAuthHeaderGenerator()
    {
        return new BasicAuthHeaderGenerator(Username, Password, FormsApplication.GetRunningApp().LocalStorageEncryptionKey);
    }
    #endregion

    #region Methods
    public virtual string GetValidationError()
    {
        if  (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) return "Please enter valid login credentials.";
        else return null;
    }
    #endregion

    #region Properties
    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            OnPropertyChanged();
        }
    }
    private string _username;

    public string Password
    {
        get => _password;
        set
        {
            if (value == _password) return;
            _password = value;
            OnPropertyChanged();
        }
    }
    private string _password;

    public string UserLabel
    {
        get => _userLabel;
        set
        {
            if (value == _userLabel) return;
            _userLabel = value;
            OnPropertyChanged();
        }
    }
    private string _userLabel;

    public long? UserId
    {
        get => _userId;
        set
        {
            if (value == _userId) return;
            _userId = value;
            OnPropertyChanged();
        }
    }
    private long? _userId;
    #endregion
}