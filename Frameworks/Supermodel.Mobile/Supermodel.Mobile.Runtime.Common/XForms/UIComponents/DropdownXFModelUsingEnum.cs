using System.Threading.Tasks;
using System.Linq;
using System;
using Supermodel.ReflectionMapper;
using System.Collections.Generic;
using System.Globalization;   

namespace Supermodel.Mobile.Runtime.Common.XForms.UIComponents;

public class DropdownXFModelUsingEnum<TEnum> : DropdownXFModel, IRMapperCustom where TEnum : struct, IConvertible
{
    #region Constructors
    public DropdownXFModelUsingEnum()
    {
        var enumValues = new List<object>();
        foreach (var item in Enum.GetValues(typeof(TEnum))) enumValues.Add(item);
        enumValues = enumValues.OrderBy(x => x.GetScreenOrder()).ToList();

        foreach (var option in enumValues) Options.Add(new EnumOption((TEnum)option));
        SelectedValue = null;
    }
    public DropdownXFModelUsingEnum(TEnum selectedEnum) : this()
    {
        SelectedEnum = selectedEnum;
    }
    #endregion

    #region Nested Options class
    public class EnumOption : Option
    {
        public EnumOption(TEnum value, string label, bool isDisabled) : base(value.ToString(CultureInfo.InvariantCulture), label, isDisabled) { }
        public EnumOption(TEnum value) : this(value, value.GetDescription(), value.IsDisabled()) { }
        public TEnum EnumValue => (TEnum)Enum.Parse(typeof(TEnum), Value);
    }
    #endregion

    #region ICstomMapper implementations
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(TEnum) && typeof(T) != typeof(TEnum?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        SelectedEnum = (TEnum?)(object)other;
        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(TEnum) && typeof(T) != typeof(TEnum?)) throw new PropertyCantBeAutomappedException($"{GetType().Name} can't be automapped to {typeof(T).Name}");
        if (typeof(T) == typeof(TEnum) && SelectedEnum == null) throw new PropertyCantBeAutomappedException(string.Format("{0} can't be automapped to {1} because {0} is null but {1} is not nullable", GetType().Name, typeof(T).Name));
        other = (T)(object)SelectedEnum; //This assignment does not do anything but we still do it for consistency
        return Task.FromResult(other);
    }
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