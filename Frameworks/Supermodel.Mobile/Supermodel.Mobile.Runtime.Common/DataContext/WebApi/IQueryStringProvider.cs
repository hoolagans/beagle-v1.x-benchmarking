namespace Supermodel.Mobile.Runtime.Common.DataContext.WebApi;

public interface IQueryStringProvider
{
    string GetQueryString(object searchBy, int? skip, int? take, string sortBy);
}