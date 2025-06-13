#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.PersistentDict;

public class PersistentDictionaryAsAppProperties : IPersistentDict
{
    #region IDicttionairy<string,object> implementation
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return Application.Current.Properties.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(KeyValuePair<string, object> item)
    {
        Application.Current.Properties.Add(item);
    }

    public void Clear()
    {
        Application.Current.Properties.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return Application.Current.Properties.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        Application.Current.Properties.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object> item)
    {
        return Application.Current.Properties.Remove(item);
    }

    public int Count => Application.Current.Properties.Count;
    public bool IsReadOnly => Application.Current.Properties.IsReadOnly;
    public void Add(string key, object value)
    {
        Application.Current.Properties.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return Application.Current.Properties.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return Application.Current.Properties.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return Application.Current.Properties.TryGetValue(key, out value);
    }

    public object this[string key]
    {
        get => Application.Current.Properties[key];
        set => Application.Current.Properties[key] = value;
    }

    public ICollection<string> Keys => Application.Current.Properties.Keys;
    public ICollection<object> Values => Application.Current.Properties.Values;
    #endregion

    #region Persistence
    public Task SaveToDiskAsync()
    {
        return Application.Current.SavePropertiesAsync();
    }
    #endregion
}