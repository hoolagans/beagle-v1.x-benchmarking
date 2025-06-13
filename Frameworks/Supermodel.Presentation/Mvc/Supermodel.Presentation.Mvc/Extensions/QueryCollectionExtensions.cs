using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Supermodel.Presentation.Mvc.Extensions;

public static class QueryCollectionExtensions
{
    #region Methods
    public static RouteValueDictionary ToRouteValueDictionary(this IQueryCollection qc)
    {
        var dict = new RouteValueDictionary();
        foreach (var key in qc.Keys)
        {
            //if (key == null) continue;
            var value = qc[key].ToString().TrimEnd().EndsWith(",") ? qc[key].ToString().RemoveStartingLast(",") : qc[key].ToString();
            dict.Add(key, value);
        }
        return dict;
    }

    public static int? GetSkipValue(this IQueryCollection qc)
    {
        var values = qc["smSkip"];
        if (values.Count == 0) return null;
        if (values.Count > 1) throw new Exception("More than one smSkip is present in query string");

        if (int.TryParse(values[0], out var value)) return value;
        else return null;
    }
    public static int? GetTakeValue(this IQueryCollection qc)
    {
        var values = qc["smTake"];
        if (values.Count == 0) return null;
        if (values.Count > 1) throw new Exception("More than one smTake is present in query string");

        if (int.TryParse(values[0], out var value)) return value;
        else return null;
    }
    public static string? GetSortByValue(this IQueryCollection qc)
    {
        var values = qc["smSortBy"];
        if (values.Count == 0) return null;
        if (values.Count > 1) throw new Exception("More than one smSortBy is present in query string");
        return values[0];
    }
    public static long? GetSelectedIdValue(this IQueryCollection qc)
    {
        var values = qc["selectedId"];
        if (values.Count == 0) return null;
        if (values.Count > 1) throw new Exception("More than one smSelectedId is present in query string");

        if (long.TryParse(values[0], out var value)) return value;
        else return null;
    }
    #endregion
}