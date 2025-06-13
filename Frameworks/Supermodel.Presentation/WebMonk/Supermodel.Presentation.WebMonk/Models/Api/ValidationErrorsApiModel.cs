using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Validations;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.WebMonk.Models.Api;

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
        var vrl = (ValidationResultList)(object)other;
            
        var modelState = new Dictionary<string, List<string>>();
        foreach (var vr in vrl)
        {
            foreach (var memberName in vr.MemberNames)
            {
                if (!modelState.ContainsKey(memberName)) modelState[memberName] = new List<string>();
                modelState[memberName].Add(vr.ErrorMessage!);
            }
        }

        foreach (var keyValuePair in modelState)
        {
            var error = new Error {Name = keyValuePair.Key};
            foreach (var errorMessage in keyValuePair.Value) error.ErrorMessages.Add(errorMessage);
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