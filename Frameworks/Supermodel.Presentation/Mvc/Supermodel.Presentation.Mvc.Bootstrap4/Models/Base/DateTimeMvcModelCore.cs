using System;
using System.Threading.Tasks;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models.Base;

public abstract class DateTimeMvcModelCore : Bs4.TextBoxMvcModel
{
    #region IRMapperCustom implemtation
    public override Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new ArgumentException("other must be of DateTime type", nameof(other));

        Value = (other != null ? other.ToString() : "")!;
        return Task.CompletedTask;
    }
        
    // ReSharper disable once RedundantAssignment
#nullable disable
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(DateTime) && typeof(T) != typeof(DateTime?)) throw new ArgumentException("other must be of DateTime type", nameof(other));

        other = (T)(object)DateTimeValue;
        return Task.FromResult(other);
    }
#nullable enable
    #endregion

    #region IComparable implementation
    public override int CompareTo(object? obj)
    {
        if (obj == null) return 1;
                    
        var valueToCompareWith = ((DateTimeMvcModelCore)obj).DateTimeValue;
        if (DateTimeValue == null && valueToCompareWith == null) return 0;
        if (DateTimeValue == null || valueToCompareWith == null) return 1;
        return DateTime.Compare(DateTimeValue.Value, valueToCompareWith.Value);
    }
    #endregion

    #region Properties
    public DateTime? DateTimeValue 
    { 
        get
        {
            if (string.IsNullOrEmpty(Value)) return null;
            if (DateTime.TryParse(Value, out var dateTime)) return dateTime;
            return null;
        }
        set => Value = value.ToString()!;
    }
    #endregion
}