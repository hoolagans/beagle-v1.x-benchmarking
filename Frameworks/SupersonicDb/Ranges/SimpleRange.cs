using System;
using System.Runtime.CompilerServices;

namespace Supersonic.Ranges;

public class SimpleRange
{
    #region Constructors
    public SimpleRange()
    {
        StartIdx = EndIdx = -1;
    }
    public SimpleRange(int startIdx, int endIdx)
    {
        if (endIdx < startIdx) throw new ArgumentException("endIdx < startIdx");
        if (startIdx < 0) throw new ArgumentException(nameof(startIdx));
        if (endIdx < 0) throw new ArgumentException(nameof(endIdx));
        StartIdx = startIdx;
        EndIdx = endIdx;
    }
    #endregion

    #region Overrides
    public override string ToString()
    {
        if (StartIdx == -1) return "[empty]";
        return $"[{StartIdx}, {EndIdx}]";
    }
    #endregion

    #region Methods
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range operator + (SimpleRange a, SimpleRange b)
    {
        return a.Union(b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Range operator * (SimpleRange a, SimpleRange b)
    {
        return a.Intersect(b);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Range Union(SimpleRange other)
    {
        if (!IntersectsWithOther(other)) return new Range(this, other);

        if (StartIdx <= other.StartIdx)
        {
            if (EndIdx <= other.EndIdx)
            {
                return new Range(new SimpleRange(StartIdx, other.EndIdx));
            }
            else
            {
                return new Range(this);
            }
        }
        else
        {
            if (EndIdx <= other.EndIdx)
            {
                return new Range(other);
            }
            else
            {
                return new Range(new SimpleRange(other.StartIdx, EndIdx));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Range Intersect(SimpleRange other)
    {
        if (!IntersectsWithOther(other)) return new Range(); //return empty range

        if (StartIdx <= other.StartIdx)
        {
            if (EndIdx <= other.EndIdx)
            {
                return new Range(new SimpleRange(other.StartIdx, EndIdx));
            }
            else
            {
                return new Range(other);
            }
        }
        else
        {
            if (EndIdx <= other.EndIdx)
            {
                return new Range(this);
            }
            else
            {
                return new Range(new SimpleRange(StartIdx, other.EndIdx));
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsSubsetOf(SimpleRange other)
    {
        if (IsEmpty) return !other.IsEmpty;
        return other.ContainsIndex(StartIdx) && other.ContainsIndex(EndIdx);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsOther(SimpleRange other)
    {
        return other.IsSubsetOf(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IntersectsWithOther(SimpleRange other)
    {
        if (IsEmpty || other.IsEmpty) return false;
        return ContainsIndex(other.StartIdx) || ContainsIndex(other.EndIdx); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ContainsIndex(int idx)
    {
        if (IsEmpty) return false;
        return StartIdx <= idx && idx <= EndIdx;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Size(int listCount)
    {
        var realEndIdx = Math.Min(EndIdx, listCount - 1);
        return realEndIdx - StartIdx + 1;
    }
    #endregion

    #region Properties
    public bool IsEmpty => StartIdx == -1;
    public int StartIdx { get; }
    public int EndIdx { get; }
    public long Length => EndIdx - StartIdx + 1;
    #endregion
}