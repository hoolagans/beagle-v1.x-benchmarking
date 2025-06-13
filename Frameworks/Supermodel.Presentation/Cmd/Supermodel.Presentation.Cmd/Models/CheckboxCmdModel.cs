using System;
using System.Threading.Tasks;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.Presentation.Cmd.Models.Base;
using Supermodel.Presentation.Cmd.Rendering;

namespace Supermodel.Presentation.Cmd.Models;

public class CheckboxCmdModel : UIComponentBase
{
    #region IRMapperCustom implemtation
    public override Task MapFromCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));

        Value = (other != null ? other.ToString() : false.ToString())!;
        return Task.CompletedTask;
    }
    // ReSharper disable once RedundantAssignment
    public override Task<T> MapToCustomAsync<T>(T other)
    {
        if (typeof(T) != typeof(bool) && typeof(T) != typeof(bool?)) throw new ArgumentException("other must be of bool type", nameof(other));

        other = (T)(object)bool.Parse(Value);
        return Task.FromResult(other);
    }
    #endregion

    #region ICmdEditor
    public override object Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        ValueBool = ConsoleExt.EditBool(ValueBool);
        return this;            
    }
    #endregion

    #region ICmdDisplayer
    public override void Display(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        CmdRender.DisplayForModel(ValueBool);
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
    public string Value { get; set; } = false.ToString();
    public bool ValueBool
    {
        get
        {
            if (string.IsNullOrEmpty(Value)) return false;
            if (bool.TryParse(Value, out var boolean)) return boolean;
            return false;
        }
        set => Value = value.ToString();
    }
    #endregion
}