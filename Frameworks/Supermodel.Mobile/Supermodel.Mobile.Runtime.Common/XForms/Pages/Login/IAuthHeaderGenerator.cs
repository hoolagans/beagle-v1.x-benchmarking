using System.Threading.Tasks;
using Supermodel.Encryptor;   

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public interface IAuthHeaderGenerator
{
    long? UserId { get; set; }
    string UserLabel { get; set; }
    AuthHeader CreateAuthHeader();

    void Clear();
    Task ClearAndSaveToPropertiesAsync();
    bool LoadFromAppProperties();
    Task SaveToAppPropertiesAsync();
}