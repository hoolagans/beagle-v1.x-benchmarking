#nullable enable

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.Mobile.Runtime.Common.PersistentDict;

public class PersistentDictionaryAsJsonFile : Dictionary<string, object>, IPersistentDict
{
    #region Constructors
    public PersistentDictionaryAsJsonFile(string fileName)
    {
        FileName = fileName;

        // ReSharper disable once VirtualMemberCallInConstructor
        var path = Path.Combine(GetDirectoryName(), FileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            JsonConvert.PopulateObject(json, this);
        }
    }
    #endregion

    #region Persistence
    public virtual string GetDirectoryName()
    {
        return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName) ?? throw new SupermodelException("Path.GetDirectoryName returned null");
    }
    public Task SaveToDiskAsync()
    {
        var json = JsonConvert.SerializeObject(this);
        var path = Path.Combine(GetDirectoryName(), FileName);
        return File.WriteAllTextAsync(path, json);
    }
    #endregion

    #region Properties
    [JsonIgnore] public string FileName { get; }
    #endregion
}