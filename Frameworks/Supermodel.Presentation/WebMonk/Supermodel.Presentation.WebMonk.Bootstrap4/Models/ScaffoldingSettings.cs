namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{
    public static class ScaffoldingSettings
    {
        //CRUD List
        public static string? ListTitleCssClass { get; set; }
        public static string? ChildListTitleCssClass { get; set; }

        public static string? CRUDListTopDivId { get; set; }
        public static string? CRUDListTopDivCssClass { get; set; }

        public static string? CRUDListAddNewCssClass { get; set; } = "btn btn-success";

        public static string? CRUDListTableId { get; set; }
        public static string? CRUDListTableCssClass { get; set; } = "table";

        public static string? CRUDListEditCssClass { get; set; } = "btn btn-success";
        public static string? CRUDListDeleteCssClass { get; set; } = "btn btn-danger";

        public static string? CRUDBinaryFileChooseCssClass { get; set; } = "choose-file-bottom-padding";
        public static string? CRUDBinaryFileDownloadCssClass { get; set; } = "btn btn-success btn-sm choose-file-top-padding";
        public static string? CRUDBinaryFileDeleteCssClass { get; set; } = "btn btn-danger btn-sm choose-file-top-padding";

        public static string? CRUDListSaveCssClass { get; set; } = "btn btn-primary";
        public static string? CRUDListCancelCssClass { get; set; } = "btn btn-success";

        //CRUDEdit
        public static string? EditTitleCssClass { get; set; }

        public static string? EditFormId { get; set; }
        public static string? EditFormFieldsetId { get; set; }

        public static string? DisplayCssClass { get; set; }
        public static string? MultiColumnDisplayCssClass { get; set; } = "mb-5 text-primary";

        public static string? EditorLabelCssClass { get; set; } = "col-sm-2 col-form-label";
        public static string? DisplayLabelCssClass { get; set; } = "col-sm-2 col-form-label display-marker";

        public static string? EditorMultiColumnLabelCssClass { get; set; } 
        public static string? DisplayMultiColumnLabelCssClass { get; set; } 

        public static string? RequiredAsteriskCssClass { get; set; }

        public static string? SaveButtonId { get; set; }
        public static string? SaveButtonCssClass { get; set; } = "btn btn-primary";

        public static string? BackButtonId { get; set; }
        public static string? BackButtonCssClass { get; set; } = "btn btn-success";

        public static string? ValidationSummaryCssClass { get; set; } = "invalid-feedback d-block";
        public static string? InlineValidationSummaryCssClass { get; set; } = "invalid-feedback d-block";

        public static string? ValidationErrorCssClass { get; set; } = "invalid-feedback d-block";
        public static string? InlineValidationErrorCssClass { get; set; } = "invalid-feedback d-block";

        //CRUD Search
        public static string? SearchTitleCssClass { get; set; }
            
        public static string? SearchFormId { get; set; }
        public static string? SearchFormFieldsetId { get; set; }

        public static string? FindButtonId { get; set; }
        public static string? FindButtonCssClass { get; set; } = "btn btn-primary";

        public static string? ResetButtonId { get; set; }
        public static string? ResetButtonCssClass { get; set; } = "btn btn-danger";

        public static string? NewSearchButtonId { get; set; }
        public static string? NewSearchButtonCssClass { get; set; } = "btn btn-success";

        public static string? SortByDropdownFormId {get; set; }
        public static string? SortByDropdownFieldsetId {get; set; }

        //Pagination
        public static string? PaginationCssClass { get; set; }

        //Accordion
        public static string? AccordionSectionTitleCss { get; set; }

        //Login
        public static string? LoginFormId { get; set; }
    }
}