using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Attributes;
using Supermodel.Presentation.Cmd.Models.Interfaces;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Cmd.Models.Base;

public abstract class UIComponentBase : IRMapperCustom, ICmdEditor, ICmdDisplayer, IUIComponentWithValue, IComparable 
{
    #region IRMapperCustom implemtation
    public abstract Task MapFromCustomAsync<T>(T other);
    public abstract Task<T> MapToCustomAsync<T>(T other);
    #endregion
        
    #region ICmdEditor implemtation
    public abstract  object? Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue);
    #endregion

    #region IDisplayTemplate implementation
    public abstract void Display(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue);
    #endregion

    #region IUIComponentWithValue implemtation 
    public abstract string ComponentValue { get; set; }
    #endregion

    #region IComparable implementation
    public virtual int CompareTo(object? obj)
    {
        if (obj == null) return 1;
                
        var valueToCompareWith = ((IUIComponentWithValue)obj).ComponentValue;
        return string.Compare(ComponentValue, valueToCompareWith, StringComparison.InvariantCulture);
    }
    #endregion

    #region ToString override
    public override string ToString()
    {
        return ComponentValue;
    }
    #endregion
}