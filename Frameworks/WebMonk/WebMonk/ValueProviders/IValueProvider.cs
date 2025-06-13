using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WebMonk.ValueProviders;

public interface IValueProvider
{
    #region Embedded Types
    public readonly struct Result
    {
        #region Constructors
        public Result(/*Type? valueProviderType,*/ object? value, bool valueMissing = false)
        {
            //if (!valueMissing && valueProviderType == null) throw new ArgumentException("If value is not missing, must provide valueProviderType");
            if (valueMissing && value != null) throw new ArgumentException("newValue must be null if valueMissing is true");
            //ValueProviderType = valueProviderType;
            Value = value;
            ValueMissing = valueMissing;
        }
        #endregion

        #region Methods
        #nullable disable
        public T UpdateInternal<T>(T oldValue)
        {
            if (ValueMissing) return oldValue;
            else return (T)Value;
        }
        public T GetCastValue<T>() => (T)Value;
        #nullable enable
        #endregion

        #region Properties
        //public Type? ValueProviderType { get; }
        public object? Value { get; }
        public bool ValueMissing { get; }
        #endregion
    }
    #endregion

    #region Methods
    List<string>? GetIndexesWithValue(string key);
    Result GetValueOrDefault<T>(string key);
    Result GetValueOrDefault(string key, Type type);
    Result GetValueOrDefault(string key);
    #endregion

    #region Properties
    ImmutableDictionary<string, object> Values { get; }
    #endregion
        
    #region Constants
    public const string FileNameSuffix = "_FileName";
    #endregion
}