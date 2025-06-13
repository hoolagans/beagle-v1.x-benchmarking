namespace Supermodel.Mobile.Runtime.Common.XForms;

using Xamarin.Forms;

public static class XFormsSettings
{
    public static int LabelFontSize { get; set; } = 14;
    public static Color LabelTextColor { get; set; } = Color.RoyalBlue;

    public static int ValueFontSize { get; set; } = 14;
    //public static Color ValueTextColor { get; set; } = Pick.ForPlatform(Color.Black, Color.White);
    public static Color ValueTextColor { get; set; } = Color.Black;

    public static Color DisabledTextColor { get; set; } = Color.LightGray;
    public static Color SwitchOnColor { get; set; } = Color.RoyalBlue;
    public static Color RequiredAsteriskColor { get; set; } = Color.Red;

    public static int MultiLineTextBoxCellHeight { get; set; } = 120;
    public static int MultiLineTextBoxReadOnlyCellHeight { get; set; } = 120;

    public static string AddNewImageFileName { get; set; }

    public static int MultiLineTextLabelHeight { get; set; } = 40;
}