using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Supermodel.Persistence.Entities;
using Supermodel.Persistence.Repository;
using Supermodel.Presentation.Mvc.Extensions;

namespace Supermodel.Presentation.Mvc.Controllers;

public static class ControllerCommon
{
    #region EmbeddedTypes
    public class ModelStateError
    {
        #region Constructors
        public ModelStateError(){}
        public ModelStateError(ModelStateEntry msEntry)
        {
            AttemptedValue = msEntry.AttemptedValue;
            foreach (var error in msEntry.Errors) ErrorMessages.Add(error.ErrorMessage);
        }
        #endregion
            
        #region Properties
        public List<string> ErrorMessages { get; set; } = new();
        public string? AttemptedValue { get; set; }
        #endregion
    }
    #endregion
        
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
    public static void AddValidationResultList(this ModelStateDictionary modelState, IEnumerable<ValidationResult> vrl, string? prefix = null)
    {
        foreach (var validationResult in vrl)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                if (string.IsNullOrEmpty(prefix)) modelState.AddModelError(memberName, validationResult.ErrorMessage!);
                else modelState.AddModelError($"{prefix}.{memberName}", validationResult.ErrorMessage!);
            }
            if (!validationResult.MemberNames.Any()) modelState.AddModelError("", validationResult.ErrorMessage!);
        }
    }
    public static string SerializeModelState(ModelStateDictionary modelState)
    {
        var errorsDict = new Dictionary<string, ModelStateError>();
        foreach (var pair in modelState)
        {
            errorsDict.Add(pair.Key, new ModelStateError(pair.Value));
        }
        return JsonConvert.SerializeObject(errorsDict);
    }
    public static ModelStateDictionary DeserializeModelState(string json)
    {
        var errorsDict = JsonConvert.DeserializeObject<Dictionary<string, ModelStateError>>(json)!;
            
        var modelState = new ModelStateDictionary();
        foreach (var error in errorsDict)
        {
            //if (error.Value == null) continue;
            foreach (var errorMsg in error.Value.ErrorMessages)
            {
                modelState.AddModelError(error.Key, errorMsg);
            }
            modelState.SetModelValue(error.Key, error.Value.AttemptedValue?.Split(','), error.Value.AttemptedValue!);
        }
        return modelState;
    }
    #endregion
}