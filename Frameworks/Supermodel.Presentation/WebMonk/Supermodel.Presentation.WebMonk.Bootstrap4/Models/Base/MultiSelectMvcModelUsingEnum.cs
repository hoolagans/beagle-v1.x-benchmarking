using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;

public abstract class MultiSelectMvcModelUsingEnum<TEnum> : MultiSelectMvcModel, IRMapperCustom where TEnum : struct, IConvertible
{
    #region Constructors
    protected MultiSelectMvcModelUsingEnum()
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Options = GetMultiSelectOptions();
    }
    #endregion

    #region Methods
    protected virtual List<Option> GetMultiSelectOptions()
    {
        var enumValues = new List<TEnum>();
        foreach (var item in Enum.GetValues(typeof(TEnum))) enumValues.Add((TEnum)item!);
        enumValues = enumValues.OrderBy(x => x.GetScreenOrder()).ToList();

        var listOfOptions = new List<Option>();
        foreach (var option in enumValues) listOfOptions.Add(new Option(option.ToString()!, option.GetDescription(), option.IsDisabled()));
        return listOfOptions;
    }
    #endregion

    #region IRMCustomMapper implementation
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        //we only allow to bind to non-nullable ulong
        if (typeof(T) != typeof(TEnum) && Enum.GetUnderlyingType(typeof(TEnum)) != typeof(ulong)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        
        //Convert from enum value to Options
        var enumValue = (TEnum)(object)other!;
        var enumUlong = (ulong)(object)enumValue;
        foreach (var option in Options)
        {
            var optionEnum = Enum.Parse<TEnum>(option.Value);
            var optionUlong = (ulong)(object)optionEnum;
            option.Selected = (enumUlong & optionUlong) != 0;
        }

        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        //we only allow to bind to non-nullable ulong
        if (typeof(T) != typeof(TEnum) && Enum.GetUnderlyingType(typeof(TEnum)) != typeof(ulong)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        
        //Convert from Options to enum value
        var enumUlong = (ulong)0;
        foreach (var option in Options)
        {
            if (option.Selected)
            {
                var optionEnum = Enum.Parse<TEnum>(option.Value);
                var optionUlong = (ulong)(object)optionEnum;
                enumUlong |= optionUlong;
            }
        }
        var enumValue = (TEnum)(object)enumUlong;
        
        other = (T)(object)enumValue;
        return Task.FromResult(other);
    }
    #endregion
}