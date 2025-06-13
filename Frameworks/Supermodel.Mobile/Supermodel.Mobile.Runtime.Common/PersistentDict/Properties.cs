using Supermodel.Mobile.Runtime.Common.Services;

#nullable enable

namespace Supermodel.Mobile.Runtime.Common.PersistentDict;

public static class Properties
{
    #region Methods
    public static IPersistentDict Dict
    {
        get
        {
            if (_dict == null)
            {
                _dict = Pick.ForPlatform<IPersistentDict>(new PersistentDictionaryAsAppProperties(),
                    new PersistentDictionaryAsAppProperties(),
                    new PersistentDictionaryAsJsonFile("props.json"));
            }
            return _dict;
        }
    }
    #endregion

    #region Propeties
    private static IPersistentDict? _dict;
    #endregion
}