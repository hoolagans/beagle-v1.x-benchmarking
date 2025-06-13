namespace BrowserEmulator;

public class SelectOption
{
    // ReSharper disable InconsistentNaming
    public SelectOption(string p_httpValue, string p_screenValue) 
        // ReSharper restore InconsistentNaming
    {
        HttpValue = p_httpValue;
        ScreenValue = p_screenValue;
    }
    // ReSharper disable InconsistentNaming
    // ReSharper disable FieldCanBeMadeReadOnly.Global
    // ReSharper disable MemberInitializerValueIgnored
    public string HttpValue = "";
    public string ScreenValue = "";
    // ReSharper restore MemberInitializerValueIgnored
    // ReSharper restore FieldCanBeMadeReadOnly.Global
    // ReSharper restore InconsistentNaming
}