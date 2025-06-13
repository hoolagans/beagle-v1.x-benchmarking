using System;
using WebMonk.ValueProviders;

namespace WebMonk.Exceptions;

public class WebMonkInvalidFormatException : WebMonkException 
{ 
    #region Constructors
    public WebMonkInvalidFormatException(IValueProvider.Result result, Type type, string key, Type valueProviderType)
    {
        Result = result;
        Type = type;
        Key = key;
        ValueProviderType = valueProviderType;
    }
    #endregion

    #region Properties
    public IValueProvider.Result Result { get; }
    public Type Type { get; }
    public string Key { get; }
    public Type ValueProviderType { get; }
    #endregion
}