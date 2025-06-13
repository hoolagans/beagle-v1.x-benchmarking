#nullable enable

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supermodel.Mobile.Runtime.Common.PersistentDict;

public interface IPersistentDict : IDictionary<string, object>
{
    Task SaveToDiskAsync();
}