using Supermodel.DataAnnotations.Misc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class LoginForm : HtmlStack
    {
        #region Constructors
        public LoginForm(object model, string? fromAction = null)
        {
            var formAttributes = new AttributesDict { { "id",  ScaffoldingSettings.LoginFormId }, {"method" , "post"}, { "enctype", "multipart/form-data"} };
            if (fromAction != null) formAttributes.Add("action", fromAction);
                
            Append(new Form(formAttributes)
            {
                Render.EditorForModel(model),
                new Input(new { name="submit-button", @class="btn btn-primary", type="submit", value="Log In" } )
            });
        }
        #endregion
    }
}