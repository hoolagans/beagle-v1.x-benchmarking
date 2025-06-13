using System;
using System.Collections.Generic;

namespace WebMonk.Misc;

public class QueryStringDict : Dictionary<string, string?>
{
    #region Constructors
    public QueryStringDict() : base(StringComparer.OrdinalIgnoreCase) { }
    public QueryStringDict(IDictionary<string, string?> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
    #endregion
}