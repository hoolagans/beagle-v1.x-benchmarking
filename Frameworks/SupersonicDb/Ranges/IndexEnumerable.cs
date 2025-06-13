using System.Collections;
using System.Collections.Generic;

namespace Supersonic.Ranges;

public class IndexEnumerable<TItem> : IEnumerable<TItem>
{
    #region Constructors
    public IndexEnumerable(Range range, List<TItem> list)
    {
        Enumerator = new IndexEnumerator<TItem>(range, list);
    }
    #endregion

    #region IEnumerable implemetation
    public IEnumerator<TItem> GetEnumerator()
    {
        return Enumerator;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion

    #region Properties
    protected Range Range { get; set; }
    protected List<TItem> List { get; set; }
    protected IndexEnumerator<TItem> Enumerator { get; set; }
    #endregion
}