using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Supermodel.Presentation.Mvc.Extensions.Gateway;

public static class SuperNamespaceGateway
{
    #region TempDataDictionary Gateway Methods
    public static SupermodelNamespaceTempDataDictionaryExtensions Super(this ITempDataDictionary tempData)
    {
        return new SupermodelNamespaceTempDataDictionaryExtensions(tempData);
    }
    #endregion
}
    
public class SupermodelNamespaceTempDataDictionaryExtensions
{
    #region Constructors
    public SupermodelNamespaceTempDataDictionaryExtensions(ITempDataDictionary tempData)
    {
        _tempData = tempData;
    }
    #endregion

    #region Methods/Properties
    public string? NextPageStartupScript
    {
        get => _tempData["sm-startupScript"]?.ToString();
        set => _tempData["sm-startupScript"] = value;
    }
    public string? NextPageAlertMessage
    {
        get => _tempData["sm-alertMessage"]?.ToString();
        set => _tempData["sm-alertMessage"] = value;
    }
    public string? NextPageModalMessage
    {
        get => _tempData["sm-modalMessage"]?.ToString();
        set => _tempData["sm-modalMessage"] = value;
    }
    #endregion

    #region Fields
    private readonly ITempDataDictionary _tempData;
    #endregion
}