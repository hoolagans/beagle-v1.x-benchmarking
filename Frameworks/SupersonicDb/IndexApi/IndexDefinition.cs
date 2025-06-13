using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Supersonic.IndexApi;

internal class IndexDefinition
{
    #region Constructor
    public class PropDefinition
    {
        public string Name { get; set; }
        public int Order { get; set; }
    }
    #endregion

    #region Methods
    public string GenerateIndexNameBasedOnIndexDefinition()
    {
        var sb = new StringBuilder();
        sb.Append(IsUnique ? "UIDX" : "IDX");
        foreach (var prop in Props.OrderBy(x => x.Order).ToList())
        {
            sb.Append($"_{prop.Name}");
        }
        return sb.ToString();
    }
    #endregion

    #region Properties
    public string Name { get; set; }
    public List<PropDefinition> Props { get; set; } = new();
    public bool IsUnique { get; set; }
    #endregion
}