using System;
using Supermodel.Presentation.Cmd.Models.Base;

namespace Supermodel.Presentation.Cmd.Models;

public class DropdownCmdModelUsingEnum<TEnum> : SingleSelectCmdModelUsingEnum<TEnum> where TEnum : struct, IConvertible
{
    #region Constructors
    public DropdownCmdModelUsingEnum(){}
    public DropdownCmdModelUsingEnum(TEnum selectedEnum) : this()
    {
        SelectedEnum = selectedEnum;
    }
    #endregion

    #region IEditorTemplate implementation
    public override object Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        return CommonDropdownEditorTemplate(this);
    }
    #endregion
}