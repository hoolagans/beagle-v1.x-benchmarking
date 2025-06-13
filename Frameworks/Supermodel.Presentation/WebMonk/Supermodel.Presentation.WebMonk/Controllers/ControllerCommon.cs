using System.Linq;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Presentation.WebMonk.Extensions;

namespace Supermodel.Presentation.WebMonk.Controllers;

public static class ControllerCommon
{
    #region Methods
    public static IQueryable<TEntity> GetItems<TEntity>() where TEntity : class, IEntity, new()
    {
        var repo = (ILinqDataRepo<TEntity>)RepoFactory.Create<TEntity>();
        return repo.Items;
    }
    public static IQueryable<TEntity> ApplySkipAndTake<TEntity>(IOrderedQueryable<TEntity> orderedItems, int? skip, int? take) where TEntity : class, IEntity, new()
    {
        if (skip != null) orderedItems = (IOrderedQueryable<TEntity>)orderedItems.Skip(skip.Value);
        if (take != null) orderedItems = (IOrderedQueryable<TEntity>)orderedItems.Take(take.Value);
        return orderedItems;
    }
    public static IOrderedQueryable<TEntity> ApplySortBy<TEntity>(IQueryable<TEntity> items, string? sortBy) where TEntity : class, IEntity, new()
    {
        if (string.IsNullOrEmpty(sortBy)) return items.OrderBy(x => x.Id);

        var columnNamesToSortBy = sortBy.Split(',');
        var itemsSorted = false;
        foreach (var trimmedColumnName in columnNamesToSortBy.Select(columnName => columnName.Trim()))
        {
            if (!trimmedColumnName.StartsWith("-"))
            {
                if (itemsSorted) items = items.ThenOrderBy(trimmedColumnName);
                else items = items.OrderBy(trimmedColumnName);
            }
            else
            {
                if (itemsSorted) items = items.ThenOrderByDescending(trimmedColumnName.Substring(1));
                else items = items.OrderByDescending(trimmedColumnName.Substring(1));
            }
            itemsSorted = true;
        }
        return (IOrderedQueryable<TEntity>)items;
    }
    #endregion
}