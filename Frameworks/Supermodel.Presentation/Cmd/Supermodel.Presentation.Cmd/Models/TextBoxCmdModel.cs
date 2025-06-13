using System;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.Presentation.Cmd.Models.Base;
using Supermodel.Presentation.Cmd.Rendering;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Cmd.Models;

public class TextBoxCmdModel : UIComponentBase
{
    #region IRMapperCustom implemtation
    public override Task MapFromCustomAsync<T>(T other)
    {
        Value = (other != null ? other.ToString() : "")!;
        InitFor<T>();
        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) == typeof(string)) 
        {
            other = (T)(object)Value;
            return Task.FromResult(other);
        }
            
        if (!string.IsNullOrEmpty(Value))
        {
            if (typeof(T) == typeof(int) || typeof(T) == typeof(int?)) other = (T)(object)int.Parse(Value);
            else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?)) other = (T)(object)uint.Parse(Value);
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?)) other = (T)(object)long.Parse(Value);
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?)) other = (T)(object)ulong.Parse(Value);
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?)) other = (T)(object)short.Parse(Value);
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?)) other = (T)(object)ushort.Parse(Value);
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?)) other = (T)(object)byte.Parse(Value);
            else if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?)) other = (T)(object)sbyte.Parse(Value);
            
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?)) other = (T)(object)double.Parse(Value);
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?)) other = (T)(object)float.Parse(Value);
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?)) other = (T)(object)decimal.Parse(Value);
            else throw new Exception($"TextBoxMvcModel.MapToCustom: Unknown type {typeof(T).GetTypeFriendlyDescription()}");
        }
        else
        {
            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>)) other = default!;
            else throw new ValidationResultException($"Cannot parse blank string into {typeof(T).GetTypeFriendlyDescription()}");
        }

        return Task.FromResult(other);
    }
    public virtual TextBoxCmdModel InitFor<T>()
    {
        Type = typeof(T);

        if (Type != typeof(string) && 
                
            Type != typeof(int) && Type != typeof(int?) &&
            Type != typeof(uint) && Type != typeof(uint?) &&

            Type != typeof(long) && Type != typeof(long?) &&
            Type != typeof(ulong) && Type != typeof(ulong?) &&
            
            Type != typeof(short) && Type != typeof(short?) &&
            Type != typeof(ushort) && Type != typeof(ushort?) &&

            Type != typeof(byte) && Type != typeof(byte?) &&
            Type != typeof(sbyte) && Type != typeof(sbyte?) &&

            Type != typeof(double) && Type != typeof(double?) &&
            Type != typeof(float) && Type != typeof(float?) &&
            Type != typeof(decimal) && Type != typeof(decimal?))
        {
            throw new Exception($"TextBoxCmdModel.InitFor: Unknown type {Type?.GetTypeFriendlyDescription()}");
        }

        //this is for fluent initialization
        return this; 
    }
    #endregion

    #region ICmdEditor implemtation
    public override object Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        if (Type == typeof(string)) { Value = ConsoleExt.EditString(Value); return this; }
            
        if (Type == typeof(int) || Type == typeof(int?)) { IntValue = ConsoleExt.EditInteger(IntValue); return this; }
        if (Type == typeof(uint) || Type == typeof(uint?)) { UIntValue = ConsoleExt.EditInteger(UIntValue); return this; }
            
        if (Type == typeof(long) || Type == typeof(long?)) { LongValue = ConsoleExt.EditInteger(LongValue); return this; }
        if (Type == typeof(ulong) || Type == typeof(ulong?)) { ULongValue = ConsoleExt.EditInteger(ULongValue); return this; }
            
        if (Type == typeof(short) || Type == typeof(short?)) { ShortValue = ConsoleExt.EditInteger(ShortValue); return this; }
        if (Type == typeof(ushort) || Type == typeof(ushort?)) { UShortValue = ConsoleExt.EditInteger(UShortValue); return this; }

        if (Type == typeof(byte) || Type == typeof(byte?)) { ByteValue = ConsoleExt.EditInteger(ByteValue); return this; }
        if (Type == typeof(sbyte) || Type == typeof(sbyte?)) { SByteValue = ConsoleExt.EditInteger(SByteValue); return this; }

        if (Type == typeof(double) || Type == typeof(double?)) { DoubleValue = ConsoleExt.EditFloat(DoubleValue); return this; }
        if (Type == typeof(float) || Type == typeof(float?)) { FloatValue = ConsoleExt.EditFloat(FloatValue); return this; }
        if (Type == typeof(decimal) || Type == typeof(decimal?)) { DecimalValue = ConsoleExt.EditFloat(DecimalValue); return this; }

        throw new Exception($"TextBoxCmdModel.Edit: Unknown type {Type?.GetTypeFriendlyDescription()}");
    }
    #endregion

    #region ICmdDisplay implementation
    public override void Display(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        CmdRender.DisplayForModel(Value);
    }
    #endregion

    #region IComparable implementation
    public override int CompareTo(object? obj)
    {
        if (obj == null) return 1;
            
        //this means this is a number
        if (Type != typeof(string))
        {
            var valueToCompareWith = ((TextBoxCmdModel)obj).DecimalValue;
            if (DecimalValue == null && valueToCompareWith == null) return 0;
            if (DecimalValue == null || valueToCompareWith == null) return 1;
            return decimal.Compare(DecimalValue.Value, valueToCompareWith.Value);
        }

        return base.CompareTo(obj);
    }
    #endregion

    #region IUIComponentWithValue implementation
    public override string ComponentValue 
    {
        get => Value;
        set => Value = value;
    }
    #endregion

    #region Properies
    public string Value { get; set; } = "";
    public int? IntValue 
    { 
        get 
        {
            if (int.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public uint? UIntValue 
    { 
        get 
        {
            if (uint.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";

    }
    public long? LongValue 
    { 
        get 
        {
            if (long.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public ulong? ULongValue 
    { 
        get 
        {
            if (ulong.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public short? ShortValue 
    { 
        get 
        {
            if (short.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public ushort? UShortValue 
    { 
        get 
        {
            if (ushort.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public byte? ByteValue 
    { 
        get 
        {
            if (byte.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public sbyte? SByteValue 
    { 
        get 
        {
            if (sbyte.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public double? DoubleValue 
    { 
        get 
        {
            if (double.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public float? FloatValue 
    { 
        get 
        {
            if (float.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }
    public decimal? DecimalValue 
    { 
        get 
        {
            if (decimal.TryParse(Value, out var val)) return val;
            return null;
        }
        set => Value = value?.ToString() ?? "";
    }

    public Type? Type { get; set; }
    #endregion
}