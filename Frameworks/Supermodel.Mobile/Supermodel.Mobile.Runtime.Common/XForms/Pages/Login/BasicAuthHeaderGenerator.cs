using Supermodel.Encryptor;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Mobile.Runtime.Common.PersistentDict;

namespace Supermodel.Mobile.Runtime.Common.XForms.Pages.Login;

public class BasicAuthHeaderGenerator : IAuthHeaderGenerator
{
    #region Constructors
    public BasicAuthHeaderGenerator(string username, string password, byte[] localStorageEncryptionKey = null)
    {
        Username = username;
        Password = password;
        LocalStorageEncryptionKey = localStorageEncryptionKey;
    }
    #endregion
				
    #region Methods
    public virtual AuthHeader CreateAuthHeader()
    {
        return HttpAuthAgent.CreateBasicAuthHeader(Username, Password);
    }

    public virtual void Clear()
    {
        Username = Password = "";
    }
    public virtual async Task ClearAndSaveToPropertiesAsync()
    {
        if (LocalStorageEncryptionKey == null) throw new SupermodelException("ClearAndSaveToPropertiesAsync(): LocalStorageEncryptionKey = null");

        Clear();
        Properties.Dict["smUsername"] = null;
        Properties.Dict["smPasswordCode"] = null;
        Properties.Dict["smPasswordIV"] = null;
        await Properties.Dict.SaveToDiskAsync();
    }
    public virtual async Task SaveToAppPropertiesAsync()
    {
        //if (Pick.RunningPlatform() == Platform.DotNetCore) throw new SupermodelException("SaveToAppPropertiesAsync() is only supported on mobile platforms");
            
        if (LocalStorageEncryptionKey == null) throw new SupermodelException("SaveToAppPropertiesAsync(): LocalStorageEncryptionKey = null");

        if  (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)) throw new SupermodelException("string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)");

        var passwordCode = EncryptorAgent.Lock(LocalStorageEncryptionKey, Password, out var passwordIV);

        Properties.Dict["smUsername"] = Username;
        Properties.Dict["smPasswordCode"] = passwordCode;
        Properties.Dict["smPasswordIV"] = passwordIV;
        Properties.Dict["smUserLabel"] = UserLabel;
        Properties.Dict["smUserId"] = UserId;
        await Properties.Dict.SaveToDiskAsync();
    }
    public virtual bool LoadFromAppProperties()
    {
        //if (Pick.RunningPlatform() == Platform.DotNetCore) throw new SupermodelException("LoadFromAppProperties() is only supported on mobile platforms");
            
        if (LocalStorageEncryptionKey == null) throw new SupermodelException("LoadFromAppProperties(): LocalStorageEncryptionKey = null");

        if (Properties.Dict.ContainsKey("smUsername") &&
            Properties.Dict.ContainsKey("smPasswordCode") &&
            Properties.Dict.ContainsKey("smPasswordIV") &&
            Properties.Dict.ContainsKey("smUserLabel") &&
            Properties.Dict.ContainsKey("smUserId"))
        {
            if (!(Properties.Dict["smUsername"] is string username)) return false;
            if (!(Properties.Dict["smPasswordCode"] is byte[] passwordCode)) return false;
            if (!(Properties.Dict["smPasswordIV"] is byte[] passwordIV)) return false;

            //User label and userId can be null
            var userLabel = Properties.Dict["smUserLabel"] as string;
            var userId = Properties.Dict["smUserId"] as long?;

            Password = EncryptorAgent.Unlock(LocalStorageEncryptionKey, passwordCode, passwordIV);
            Username = username;
            UserLabel = userLabel;
            UserId = userId;
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
				
    #region Properties
    public long? UserId { get; set; }
    public string UserLabel { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    private byte[] LocalStorageEncryptionKey { get; }
    #endregion
}