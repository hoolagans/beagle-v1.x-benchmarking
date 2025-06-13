using WebMonk.ValueProviders;

namespace WebMonk.Context;

public interface IRouteManager
{
    string BaseUrl { get; }
    string LocalPath { get; }
    string QueryString { get; }
    string[] LocalPathParts { get; }
    string OverridenHttpMethod { get; set; }
    string LocalPathWithQueryString { get; }
    string LocalPathWithQueryStringMinusSelectedId { get; }

    string GetControllerFromRoute();
    string? GetAction();

    RouteValueProvider RouteValueProvider { get; }
}