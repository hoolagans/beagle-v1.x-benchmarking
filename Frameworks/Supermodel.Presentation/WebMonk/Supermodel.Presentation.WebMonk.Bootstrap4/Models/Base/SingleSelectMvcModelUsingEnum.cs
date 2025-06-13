using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models.Base;

public abstract class SingleSelectMvcModelUsingEnum<TEnum> : SingleSelectMvcModel where TEnum : struct, IConvertible
{
    #region Nested Options class
    public class EnumOption : Option
    {
        #region Constructors
        public EnumOption(TEnum value) : this(value, value.GetDescription(), value.IsDisabled()) { }
        public EnumOption(TEnum value, string label, bool isDisabled) : base(value.ToString(CultureInfo.InvariantCulture), label, isDisabled) { }
        #endregion

        #region Methods
        public TEnum EnumValue => (TEnum)Enum.Parse(typeof(TEnum), Value);
        #endregion
    }
    #endregion
        
    #region Constructors
    protected SingleSelectMvcModelUsingEnum()
    {
        var enumValues = new List<object>();
        foreach (var item in Enum.GetValues(typeof(TEnum))) enumValues.Add(item!);
        enumValues = enumValues.OrderBy(x => x.GetScreenOrder()).ToList();

        foreach (var option in enumValues) Options.Add(new EnumOption((TEnum)option));
        SelectedValue = null;
    }

    protected SingleSelectMvcModelUsingEnum(TEnum selectedEnum) : this()
    {
        SelectedEnum = selectedEnum;
    }
    #endregion

    #region IRMCstomMapper implementations
    public override Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(TEnum) && typeof(T) != typeof(TEnum?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        SelectedEnum = (TEnum?)(object?)other;
        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    #nullable disable
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(TEnum) && typeof(T) != typeof(TEnum?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        if (SelectedEnum == null && !(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1} because {0} is null but {1} is not nullable", GetType().Name, typeof(T).Name));
        other = (T)(object)SelectedEnum; //This assignment does not do anything but we still do it for consistency
        return Task.FromResult(other);
    }
    #nullable enable
    #endregion

    #region Properties
    public TEnum? SelectedEnum
    {
        get
        {
            if (string.IsNullOrEmpty(SelectedValue)) return null;
            return (TEnum)Enum.Parse(typeof(TEnum), SelectedValue);
        }
        set => SelectedValue = value == null ? "" : ((TEnum)value).ToString(CultureInfo.InvariantCulture);
    }
    #endregion
}