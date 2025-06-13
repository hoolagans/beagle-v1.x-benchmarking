namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class SimpleSearchMvcModel : MvcModel
    {
        #region Properties
        public TextBoxMvcModel SearchTerm { get; set; } = new();
        #endregion
    }
}