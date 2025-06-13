using Supermodel.DataAnnotations.Validations;
using System;
using System.Threading.Tasks;
using Supermodel.ReflectionMapper;    

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents.Base;

public static class SingleCellUIComponentForTextXFModelCommonLib
{
    #region ICustomMapper Common Implemtation for Text
    public static Task MapFromCustomAsync<T>(IHaveTextProperty me, T other)
    {
        if (typeof(T) != typeof(string) && typeof(T) != typeof(int)  && typeof(T) != typeof(long)  && typeof(T) != typeof(double)  && typeof(T) != typeof(float)
            && typeof(T) != typeof(int?) && typeof(T) != typeof(long?) && typeof(T) != typeof(double?) && typeof(T) != typeof(float?))
        {
            throw new PropertyCantBeAutomappedException($"{me.GetType().Name} can't be automapped to {typeof(T).Name}");
        }

        var domainObjStr = other?.ToString();
        if (domainObjStr != null) me.Text = domainObjStr;

        return Task.CompletedTask;
    }
    public static Task<T> MapToCustomAsync<T>(IHaveTextProperty me, T other)
    {
        //If both Text and domain property are blank, return the current state
        var domainObjStr = other?.ToString();
        if (string.IsNullOrEmpty(me.Text) && string.IsNullOrEmpty(domainObjStr)) return Task.FromResult(other);
            
        if (typeof(T) == typeof(string)) return Task.FromResult((T)(object)me.Text);

        try
        {
            if (typeof(T) == typeof(int)) return Task.FromResult((T)(object)int.Parse(me.Text));
            if (typeof(T) == typeof(int?)) return Task.FromResult((T)(object)(string.IsNullOrEmpty(me.Text) ? null : int.Parse(me.Text)));

            if (typeof(T) == typeof(long)) return Task.FromResult((T)(object)long.Parse(me.Text));
            if (typeof(T) == typeof(long?)) return Task.FromResult((T)(object)(string.IsNullOrEmpty(me.Text) ? null : long.Parse(me.Text)));
            
            if (typeof(T) == typeof(double)) return Task.FromResult((T)(object)double.Parse(me.Text));
            if (typeof(T) == typeof(double?)) return Task.FromResult((T)(object)(string.IsNullOrEmpty(me.Text) ? null : double.Parse(me.Text)));

            if (typeof(T) == typeof(float)) return Task.FromResult((T)(object)float.Parse(me.Text));
            if (typeof(T) == typeof(float?)) return Task.FromResult((T)(object)(string.IsNullOrEmpty(me.Text) ? null : float.Parse(me.Text)));
        }
        catch (FormatException)
        {
            throw new ValidationResultException("Invalid Format");
        }
        catch(OverflowException)
        {
            throw new ValidationResultException("Invalid Format");
        }

        throw new PropertyCantBeAutomappedException($"{me.GetType().Name} can't be automapped to {typeof(T).Name}");
    }
    #endregion
}