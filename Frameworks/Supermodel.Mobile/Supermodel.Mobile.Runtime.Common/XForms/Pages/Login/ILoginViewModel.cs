using System.ComponentModel;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public interface ILoginViewModel : INotifyPropertyChanged
{
    string GetValidationError();
}