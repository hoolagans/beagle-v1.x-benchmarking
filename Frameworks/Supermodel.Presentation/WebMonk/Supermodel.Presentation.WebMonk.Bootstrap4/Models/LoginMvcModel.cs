using System.ComponentModel.DataAnnotations;
using Supermodel.Presentation.WebMonk.Models.Mvc;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class LoginMvcModel : MvcModel, ILoginMvcModel
    {
        public TextBoxMvcModel Username { get; set; } = new();
        public PasswordTextBoxMvcModel Password { get; set; } = new();

        [ScaffoldColumn(false)] public string UsernameStr
        {
            get => Username.Value;
            set => Username.Value = value;
        }
        [ScaffoldColumn(false)] public string PasswordStr
        {
            get => Password.Value;
            set => Password.Value = value;
        }
    }
}