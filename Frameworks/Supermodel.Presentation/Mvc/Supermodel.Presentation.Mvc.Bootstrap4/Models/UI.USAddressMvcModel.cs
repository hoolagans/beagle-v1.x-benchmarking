using System.ComponentModel.DataAnnotations;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{
    public class USAddressMvcModel : ValueObjectMvcModel
    {
        #region Properties
        public TextBoxMvcModel Street { get; set; } = new();
        public TextBoxMvcModel City { get; set; } = new();
        public TextBoxMvcModel State { get; set; } = new();
        public TextBoxMvcModel Zip { get; set; } = new();
        #endregion
    }

    public class USAddressRequiredMvcModel : ValueObjectMvcModel
    {
        #region Properties
        [Required] public TextBoxMvcModel Street { get; set; } = new();
        [Required] public TextBoxMvcModel City { get; set; } = new();
        [Required] public TextBoxMvcModel State { get; set; } = new();
        [Required] public TextBoxMvcModel Zip { get; set; } = new();
        #endregion
    }
}