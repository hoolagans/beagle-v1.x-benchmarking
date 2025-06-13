using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Views;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Views;

public abstract class SimpleAuthMvcView: SimpleAuthViewBase<Bs4.LoginMvcModel>
{
    #region Action View Methods
    public override IGenerateHtml RenderLogin(Bs4.LoginMvcModel model)
    {
        return ApplyToDefaultLayout(new Bs4.LoginForm(model));
    }
    #endregion
}