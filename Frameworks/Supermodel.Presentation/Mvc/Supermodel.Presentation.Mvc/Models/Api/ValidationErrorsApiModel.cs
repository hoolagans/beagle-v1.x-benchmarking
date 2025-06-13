using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Models.Api;

public class ValidationErrorsApiModel : List<ValidationErrorsApiModel.Error>, IRMapperCustom
{
    #region Embedded Types
    public class Error
    {
        #region Properties
        public string Name { get; set; } = "";
        // ReSharper disable once CollectionNeverQueried.Global
        public List<string> ErrorMessages { get; set; } = new();
        #endregion
    }
    #endregion

    #region IRMapperCustom implementation
    public Task MapFromCustomAsync<T>(T other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        var modelState = (ModelStateDictionary)(object)other;
        foreach (var keyValuePair in modelState)
        {
            var error = new Error {Name = keyValuePair.Key};
            foreach (var errorMessage in keyValuePair.Value.Errors.Select(x => x.ErrorMessage)) error.ErrorMessages.Add(errorMessage);
            Add(error);
        }
        return Task.CompletedTask;
    }
    public Task<T> MapToCustomAsync<T>(T other)
    {
        throw new InvalidOperationException();
    }
    #endregion

    #region Properties
    public string Message { get; set; } = "Validation Error(s)";
    #endregion
}