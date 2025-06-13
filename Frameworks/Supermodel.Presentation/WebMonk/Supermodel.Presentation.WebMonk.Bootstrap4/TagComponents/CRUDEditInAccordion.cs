using System.Collections.Generic;
using Supermodel.DataAnnotations.Enums;
using Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;
using Supermodel.Presentation.WebMonk.Models;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;
using WebMonk.Rendering.Templates;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public class CRUDEditInAccordion : AccordionBase
    {
        #region Constructors
        public CRUDEditInAccordion(IEditorTemplate model, string accordionId, IEnumerable<AccordionPanel> panels, string pageTitle, bool readOnly = false, bool skipBackButton = false, bool skipHeaderAndFooter = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always) :
            this(model, accordionId, panels, new Txt(pageTitle), readOnly, skipBackButton, skipHeaderAndFooter, validationSummaryVisible)
        { }

        public CRUDEditInAccordion(IEditorTemplate model, string accordionId, IEnumerable<AccordionPanel> panels, IGenerateHtml? pageTitle = null, bool readOnly = false, bool skipBackButton = false, bool skipHeaderAndFooter = false, ValidationSummaryVisible validationSummaryVisible = ValidationSummaryVisible.Always)
        {
            if (!skipHeaderAndFooter) AppendAndPush(new CRUDEditContainer((IViewModelForEntity)model, pageTitle, readOnly, skipBackButton, validationSummaryVisible));

            AppendAndPush(new Div(new { id=accordionId }));
            foreach (var panel in panels)
            {
                var body = model.EditorTemplate(panel.ScreenOrderFrom, panel.ScreenOrderTo).DisableAllControlsIf(readOnly);
                // ReSharper disable once VirtualMemberCallInConstructor
                Append(GetAccordionSection(accordionId, panel, body));
            }
            Pop<Div>();

            if (!skipHeaderAndFooter) Pop<CRUDEditContainer>();

            //Remove disabling fieldset for accordion, for accordion we disable each individual panel
            // ReSharper disable once VirtualMemberCallInConstructor
            var fieldset = FirstWhere(x => x.TagType == "fieldset");
            fieldset.Attributes.Remove("disabled");
        }
        #endregion
    }
}