namespace WebMonk.Extensions;

public static class StringExt
{
    #region Methods
    public static string ToHtmlId(this string me)
    {
        return me;
        //return me.Replace(".", "-");
    }
    public static string ToHtmlName(this string me)
    {
        return me;
        //return me.Replace("-", ".");
    }
    #endregion
}