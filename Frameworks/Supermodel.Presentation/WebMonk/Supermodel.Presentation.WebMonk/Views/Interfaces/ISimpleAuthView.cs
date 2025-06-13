using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Views.Interfaces;

public interface ISimpleAuthView<in TLoginMvcModel>  where TLoginMvcModel : class, ILoginMvcModel, new()
{
    IGenerateHtml RenderLogin(TLoginMvcModel model);
}