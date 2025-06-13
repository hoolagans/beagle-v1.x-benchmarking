namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class AccordionPanel(string elementId, string title, int screenOrderFrom, int screenOrderTo, bool expanded)
    {
        public string ElementId { get; } = elementId;
        public string Title { get; } = title;
        public int ScreenOrderFrom { get;} = screenOrderFrom;
        public int ScreenOrderTo { get; } = screenOrderTo;
        public bool Expanded { get; } = expanded;
    }
}