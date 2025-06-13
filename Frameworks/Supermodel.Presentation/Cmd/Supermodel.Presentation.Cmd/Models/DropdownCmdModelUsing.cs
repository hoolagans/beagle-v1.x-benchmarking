using Supermodel.Presentation.Cmd.Models.Base;

namespace Supermodel.Presentation.Cmd.Models;

public class DropdownCmdModelUsing<TMvcModel> : SingleSelectMvcModelUsing<TMvcModel> where TMvcModel : CmdModelForEntityCore
{
    #region IEditorTemplate implementation
    public override object Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        return CommonDropdownEditorTemplate(this);
    }
    #endregion
}