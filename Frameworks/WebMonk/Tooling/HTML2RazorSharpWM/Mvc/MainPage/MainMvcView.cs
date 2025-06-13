using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Views;

namespace HTML2RazorSharpWM.Mvc.MainPage;

public class MainMvcView : MvcView
{
    #region Methods
    public IGenerateHtml RenderIndex()
    {
        var html = new Tags
        {
            new Div
            {
                new H2
                {
                    new Txt("Input (HTML)")
                },
                new Textarea(new { id="input-text-area" })
            },

            new Br(),
            new Input(new { id="sort-attributes", type="checkbox", value=true, }),
            new Label(new { @for="sort-attributes", })
            {
                new Txt("Sort Attributes"),
            },
            new Br(),
            new Input(new { id="generate-invalid-tags", type="checkbox", value=true, }),
            new Label(new { @for="generate-invalid-tags", })
            {
                new Txt("Generate Invalid Tags"),
            },
            new Br(),
            new Div
            {
                new H2
                {
                    new Txt("Output (RazorSharp)")
                },
                new Textarea( new { id="output-text-area", @readonly="" })
            }
        };

        return ApplyToDefaultLayout(html);
    }
    #endregion
}