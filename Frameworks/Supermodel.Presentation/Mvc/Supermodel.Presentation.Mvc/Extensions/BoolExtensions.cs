namespace Supermodel.Presentation.Mvc.Extensions;

public static class BoolExtensions
{
    public static string ToYesNo(this bool? me)
    {
        if (me == null) return "";
        return me.Value.ToYesNo();
    }

    public static string ToYesNo(this bool me)
    {
        return me ? "Yes" : "No";
    }
}