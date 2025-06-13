using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;

public abstract class AccordionBase : HtmlSnippet
{
    #region Protected Helpers
    protected virtual IGenerateHtml GetAccordionSection(string accordionId, Bs4.AccordionPanel panel, IGenerateHtml body)
    {
        return new Div(new { @class="card" })
        {
            new Div(new { @class="card-header", id=$"heading_{panel.ElementId}" })
            {
                new H5(new { @class="mb-0" })
                {
                    new Button(new { type="button", @class="btn btn-link", data_toggle="collapse", data_target=$"#collapse_{panel.ElementId}", aria_expanded=panel.Expanded, aria_controls=$"collapse_{panel.ElementId}"})
                    {
                        new H5(new { @class=Bs4.ScaffoldingSettings.AccordionSectionTitleCss }) { new Txt(panel.Title) }
                    }
                }
            },
            new Div(new { id=$"collapse_{panel.ElementId}", @class=$"collapse {(panel.Expanded? "show" : "")}", aria_labelledby=$"heading_{panel.ElementId}", data_parent=$"#{accordionId}" })
            {
                new Div(new { @class="card-body" })
                { 
                    body
                }
            }
        };
    }
    #endregion
}