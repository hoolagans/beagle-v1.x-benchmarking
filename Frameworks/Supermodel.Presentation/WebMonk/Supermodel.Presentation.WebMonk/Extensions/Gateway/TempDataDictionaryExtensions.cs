using WebMonk.Session;

namespace Supermodel.Presentation.WebMonk.Extensions.Gateway;

public static class SuperNamespaceGateway
{
    #region TempDataDictionary Gateway Methods
    public static SupermodelNamespaceTempDataDictionaryExtensions Super(this TempDataDictionary tempData)
    {
        return new SupermodelNamespaceTempDataDictionaryExtensions(tempData);
    }
    #endregion
}
    
public class SupermodelNamespaceTempDataDictionaryExtensions(TempDataDictionary tempData)
{
    #region Methods/Properties
    public string? NextPageStartupScript
    {
        get => (string?)tempData["sm-startupScript"];
        set => tempData["sm-startupScript"] = value;
    }
    public string? NextPageAlertMessage
    {
        get => (string?)tempData["sm-alertMessage"];
        set => tempData["sm-alertMessage"] = value;
    }
    public string? NextPageModalMessage
    {
        get => (string?)tempData["sm-modalMessage"];
        set => tempData["sm-modalMessage"] = value;
    }
    #endregion
}