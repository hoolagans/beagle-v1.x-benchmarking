using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Supersonic.Ranges;

public class IndexEnumerator<TItem> : IEnumerator<TItem>
{
    #region Constructors
    public IndexEnumerator(Range range, List<TItem> list)
    {
        Range = range;
        List = list;
        Reset();
    }
    #endregion

    #region IEnumerator implemtation
    public void Reset()
    {
        CurrentSimpleRange = 0;
        CurrentSimpleRangeIndex = -1;
    }

    public TItem Current
    {
        get
        {
            var index = Range.SimpleRanges[CurrentSimpleRange].StartIdx + CurrentSimpleRangeIndex;
            return List[index];
        }
    }
    object IEnumerator.Current => Current;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        var simpleRanges = Range.SimpleRanges;
        if (CurrentSimpleRange >= simpleRanges.Length) return false;
        if (CurrentSimpleRangeIndex >= simpleRanges[CurrentSimpleRange].EndIdx - simpleRanges[CurrentSimpleRange].StartIdx)
        {
            if (CurrentSimpleRange + 1 >= simpleRanges.Length) return false;

            //Check that we don't step out of the array boundry
            if (Range.SimpleRanges[CurrentSimpleRange + 1].StartIdx >= List.Count) return false;

            CurrentSimpleRangeIndex = 0;
            CurrentSimpleRange++;
            return true;
        }

        //Check that we don't step out of the array boundry
        if (Range.SimpleRanges[CurrentSimpleRange].StartIdx + CurrentSimpleRangeIndex + 1 >= List.Count) return false;

        CurrentSimpleRangeIndex++;
        return true;
    }

    public void Dispose(){} //Do nothing
    #endregion

    #region Properties
    protected Range Range { get; set; }
    protected List<TItem> List { get; set; }

    protected int CurrentSimpleRange { get; set; }
    protected int CurrentSimpleRangeIndex { get; set; }
    #endregion

}