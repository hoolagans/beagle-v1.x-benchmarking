using Supermodel.DataAnnotations.Exceptions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents;

public class ModalContainer : HtmlContainerSnippet
{
    #region Embedded Types
    public enum Width { Small, Medium, Large }
    #endregion
        
    #region Constructors
    public ModalContainer(string dialogId, IGenerateHtml? title = null, IGenerateHtml? footer = null, Width width = Width.Medium, bool verticallyCentered = false, string backgroundColor = "white")
    {
        InnerContent = new Tags();

        string modalDialogCssClass;
        switch (width)
        {
            case Width.Small:
            {
                modalDialogCssClass = "modal-dialog modal-sm";
                break;
            }
            case Width.Medium:
            {
                modalDialogCssClass = "modal-dialog";
                break;
            }
            case Width.Large:
            {
                modalDialogCssClass = "modal-dialog modal-lg";
                break;
            }
            default:
            {
                throw new SupermodelException($"Unknown Width: {width}");
            }
        }
        if (verticallyCentered) modalDialogCssClass += " modal-dialog-centered";

        AppendAndPush(new Div(new { @class = "modal fade", id = dialogId, tabindex = "-1", role = "dialog", aria_labelledby = "exampleModalLabel", aria_hidden = "true" }));
        AppendAndPush(new Div(new { @class = modalDialogCssClass, role = "document" }));
        AppendAndPush(new Div(new { @class = "modal-content", style = $"background-color: {backgroundColor};" }));
            
        //header
        if (title != null)
        {
            AppendAndPush(new Div(new { @class = "modal-header" }));
            Append(new H5(new { @class = "modal-title" }) { title });
            Append(new Button(new { type = "button", @class = "close", data_dismiss = "modal", aria_label = "Close" })
            {
                new Span(new { aria_hidden="true"})
                {
                    new Txt("×")
                }
            });
            Pop<Div>();
        }

        //body
        Append(new Div(new { @class = "modal-body" }) { InnerContent });

        if (footer != null) Append(new Div(new { @class = "modal-footer" }) { footer });

        //footer
        Pop<Div>();
        Pop<Div>();
        Pop<Div>();
    }
    #endregion
}