namespace WebMonk.Rendering.Views;

public class SelectListItem
{
    #region Constructors
    public SelectListItem(string value, string label, string? optionGroup = null)
    {
        Value = value;
        Label = label;
        OptionGroup = optionGroup;
    }
    #endregion

    #region Properties
    public string Value { get; }
    public string Label { get; }
    public string? OptionGroup { get; }
    #endregion

    #region Static constants
    public static SelectListItem Empty { get; } = new("", "");
    #endregion
}