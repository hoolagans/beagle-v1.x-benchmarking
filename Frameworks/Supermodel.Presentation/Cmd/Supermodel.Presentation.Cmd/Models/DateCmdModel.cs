using System;
using System.Threading.Tasks;
using Supermodel.Presentation.Cmd.ConsoleOutput;
using Supermodel.Presentation.Cmd.Models.Base;

namespace Supermodel.Presentation.Cmd.Models;

public class DateCmdModel : DateTimeCmdModelCore
{
    #region Constructors
    public DateCmdModel()
    {
        Type = typeof(DateTime);
    }
    #endregion

    #region ICmdEditor
    public override object Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        DateTimeValue = ConsoleExt.EditDate(DateTimeValue, Placeholder);
        return this;            
    }
    #endregion

    #region ICmdDisplay
    public override void Display(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue)
    {
        if (DateTimeValue != null) Console.Write(DateTimeValue.Value.ToString("MM/dd/yyyy"));
    }
    #endregion

    #region IRMapperCustom implemtation
    public override async Task MapFromCustomAsync<T>(T other)
    {
        await base.MapFromCustomAsync(other).ConfigureAwait(false);
                
        //Set correct format
        if (DateTimeValue != null) Value = DateTimeValue.Value.ToString("MM/dd/yyyy");
    }
    #endregion

    #region Properties
    protected static StringWithColor Placeholder { get; } = new("MM/dd/YYYY", ConsoleColor.DarkYellow);
    #endregion
}