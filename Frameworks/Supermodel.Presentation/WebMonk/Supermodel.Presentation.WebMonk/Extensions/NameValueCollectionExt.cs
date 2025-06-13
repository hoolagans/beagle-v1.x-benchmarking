using System;
using System.Collections.Specialized;

namespace Supermodel.Presentation.WebMonk.Extensions;

public static class NameValueCollectionExt
{
    public static int? GetSkipValue(this NameValueCollection me)
    {
        var values = me.GetValues("smSkip");
        if (values == null) return null;
        if (values.Length == 0) return null;
        if (values.Length > 1) throw new Exception("More than one smSkip is present in query string");

        if (int.TryParse(values[0], out var value)) return value;
        else return null;
    }
    public static int? GetTakeValue(this NameValueCollection me)
    {
        var values = me.GetValues("smTake");
        if (values == null) return null;
        if (values.Length == 0) return null;
        if (values.Length > 1) throw new Exception("More than one smTake is present in query string");

        if (int.TryParse(values[0], out var value)) return value;
        else return null;
    }
    public static string? GetSortByValue(this NameValueCollection me)
    {
        var values = me.GetValues("smSortBy");
        if (values == null) return null;
        if (values.Length == 0) return null;
        if (values.Length > 1) throw new Exception("More than one smSortBy is present in query string");
        return values[0];
    }
    public static long? GetSelectedIdValue(this NameValueCollection me)
    {
        var values = me.GetValues("selectedId");
        if (values == null) return null;
        if (values.Length == 0) return null;
        if (values.Length > 1) throw new Exception("More than one smSelectedId is present in query string");

        if (long.TryParse(values[0], out var value)) return value;
        else return null;
    }
}