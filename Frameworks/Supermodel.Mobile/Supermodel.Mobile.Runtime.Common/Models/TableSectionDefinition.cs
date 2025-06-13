
namespace Supermodel.Mobile.Runtime.Common.Models;

public class TableSectionDefinition
{
    #region Constructors
    public TableSectionDefinition() { }
    public TableSectionDefinition(string title, int screenOrderFrom, int screenOrderTo)
    {
        Title = title;
        ScreenOrderFrom = screenOrderFrom;
        ScreenOrderTo = screenOrderTo;
    }
    #endregion

    #region Properties
    public string Title { get; set; }
    public int ScreenOrderFrom { get; set; }
    public int ScreenOrderTo { get; set; }
    #endregion
}