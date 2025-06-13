using System;

namespace Supersonic;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ListIndexAttribute : Attribute
{
    #region Constructors
    public ListIndexAttribute() 
    {
        Name = null;
    }
    public ListIndexAttribute(string name)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
        Name = name;
    }
    public ListIndexAttribute(string name, int order)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
        if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
        Name = name;
        _order = order;
    }
    public ListIndexAttribute(string name, int order, bool isUnique)
    {
        if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));
        if (order < 0) throw new ArgumentOutOfRangeException(nameof(order));
        Name = name;
        _order = order;
        IsUnique = isUnique;
    }
    #endregion

    #region Properties
    public string Name { get; set; }
    public bool IsUnique { get; set; }
    public int Order
    {
        get => _order;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _order = value;
        }
    }
    private int _order = -1;
    #endregion
}