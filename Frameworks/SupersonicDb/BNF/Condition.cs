using System.Collections.Generic;

namespace Supersonic.BNF;

internal class Condition
{
    #region Properties
    public List<Term> Terms { get; set; } = new();
    #endregion
}