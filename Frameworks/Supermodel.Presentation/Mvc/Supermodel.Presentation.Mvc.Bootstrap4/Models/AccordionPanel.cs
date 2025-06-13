namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class AccordionPanel 
    {
        public AccordionPanel(string elementId, string title, int screenOrderFrom, int screenOrderTo, bool expanded)
        {
            ElementId = elementId;
            Title = title;
            ScreenOrderFrom = screenOrderFrom;
            ScreenOrderTo = screenOrderTo;
            Expanded = expanded;
        }
        
        public string ElementId { get; }
        public string Title { get; }
        public int ScreenOrderFrom { get;}
        public int ScreenOrderTo { get; }
        public bool Expanded { get; }
    }
}