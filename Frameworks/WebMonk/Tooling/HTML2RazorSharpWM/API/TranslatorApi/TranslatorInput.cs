namespace HTML2RazorSharpWM.API.TranslatorApi;

public class TranslatorInput
{
    #region Properties
    public string Html { get; set; } = string.Empty;
    public bool GenerateInvalidTags { get; set; }
    public bool SortAttributes { get; set; }
    #endregion
}