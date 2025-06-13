using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Supermodel.DataAnnotations.Attributes;

namespace Supermodel.DataAnnotations.Validations;

public static class AsyncValidator
{
    #region Embedded Types
    private class ValidationError
    {
        #region Constructors
        internal ValidationError(ValidationAttribute? validationAttribute, object? value, ValidationResult validationResult)
        {
            _validationAttribute = validationAttribute;
            ValidationResult = validationResult;
            _value = value;
        }
        #endregion

        #region Methods
        internal ValidationResult ValidationResult { get; }
        internal void ThrowValidationException() => throw new ValidationException(ValidationResult, _validationAttribute, _value);
        #endregion

        #region Properties
        private readonly object? _value;
        private readonly ValidationAttribute? _validationAttribute;
        #endregion
    }
    #endregion

    #region Methods
    public static async Task<bool> TryValidateObjectAsync(object instance, ValidationContext validationContext, ICollection<ValidationResult> validationResults, bool validateAllProperties = true, bool breakOnFirstError = false)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (validationContext != null && instance != validationContext.ObjectInstance) throw new ArgumentException("Instance Must Match Validation Context Instance", nameof(instance));
 
        var result = true;
 
        var errors = await GetObjectValidationErrorsAsync(instance, validationContext, validateAllProperties, breakOnFirstError);
        foreach (var err in errors)
        {
            result = false;
            if (err == null) throw new Exception("err == null: this should never happen");

            //We only add non-duplicate errors
            if (validationResults.All(x => !AreEqual(x, err.ValidationResult))) validationResults.Add(err.ValidationResult);
        }
 
        return result;
    }
    public static async Task ValidateObjectAsync(object instance, ValidationContext validationContext, bool validateAllProperties = false)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));
        if (instance != validationContext.ObjectInstance) throw new ArgumentException("Instance Must Match Validation Context Instance", nameof(instance));
 
        (await GetObjectValidationErrorsAsync(instance, validationContext, validateAllProperties, false)).FirstOrDefault()?.ThrowValidationException();
    }
    #endregion

    #region Private Helpers
    public static bool AreEqual(ValidationResult a, ValidationResult b)
    {
        if (a.ErrorMessage != b.ErrorMessage) return false;
            
        var aMemberNames = a.MemberNames.ToArray();
        var bMemberNames = b.MemberNames.ToArray();

        if (aMemberNames.Length != bMemberNames.Length) return false;
        for(var i = 0; i < aMemberNames.Length; i++)
        {
            if (aMemberNames[i] != bMemberNames[i]) return false;
        }
        return true;
    }
    private static ValidationContext CreateValidationContext(object instance, ValidationContext validationContext)
    {
        Debug.Assert(validationContext != null);
 
        // Create a new context using the existing ValidationContext that acts as an IServiceProvider and contains our existing items.
        var context = new ValidationContext(instance, validationContext, validationContext.Items);
        return context;
    }
    private static async Task<IEnumerable<ValidationError>> GetObjectValidationErrorsAsync(object instance, ValidationContext? validationContext, bool validateAllProperties, bool breakOnFirstError)
    {
        Debug.Assert(instance != null);
 
        if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));
 
        // Step 1: Validate the object properties' validation attributes
        var errors = new List<ValidationError>();
        errors.AddRange(GetObjectPropertyValidationErrors(instance, validationContext, validateAllProperties, breakOnFirstError));
 
        // We only proceed to Step 2 if there are no errors
        if (errors.Any()) return errors;
 
        // Step 2: Validate the object's validation attributes
        var attributes = _store.GetTypeValidationAttributes(validationContext);
        errors.AddRange(GetValidationErrors(instance, validationContext, attributes, breakOnFirstError));
 
        // We only proceed to Step 3 if there are no errors
        if (errors.Any()) return errors;
 
        // Step 3: Test for IValidatableObject implementation            
        if (instance is IValidatableObject validatable)
        {
            var results = validatable.Validate(validationContext);
 
            foreach (var result in results.Where(r => r != ValidationResult.Success))
            {
                errors.Add(new ValidationError(null, instance, result));
            }
        }
 
        // Step 4: Test for IAsyncValidatableObject implementation            
        if (instance is IAsyncValidatableObject asyncValidatable)
        {
            var results = await asyncValidatable.ValidateAsync(validationContext);
 
            foreach (var result in results.Where(r => r != ValidationResult.Success))
            {
                errors.Add(new ValidationError(null, instance, result));
            }
        }

        return errors;
    }
    private static IEnumerable<ValidationError> GetObjectPropertyValidationErrors(object instance, ValidationContext validationContext, bool validateAllProperties, bool breakOnFirstError)
    {
        var properties = GetPropertyValues(instance, validationContext);
        var errors = new List<ValidationError>();
 
        foreach (var property in properties)
        {
            // get list of all validation attributes for this property
            var attributes = _store.GetPropertyValidationAttributes(property.Key);
 
            if (validateAllProperties)
            {
                // validate all validation attributes on this property
                errors.AddRange(GetValidationErrors(property.Value, property.Key, attributes, breakOnFirstError));
            }
            else
            {
                // only validate the Required attributes
                var reqAttr = attributes.OfType<RequiredAttribute>().FirstOrDefault();
                if (reqAttr != null)
                {
                    // Note: we let the [Required] attribute do its own null testing,
                    // since the user may have sub-classed it and have a deeper meaning to what 'required' means
                    var valueToCheck = property.Value;
                    if (valueToCheck is IUIComponentWithValue componentWithValue) valueToCheck = componentWithValue.ComponentValue;

                    var validationResult = reqAttr.GetValidationResult(valueToCheck, property.Key);
                    if (validationResult != ValidationResult.Success)
                    {
                        errors.Add(new ValidationError(reqAttr, property.Value, validationResult!));
                    }
                }
            }
 
            if (breakOnFirstError && errors.Any()) break;
        }
 
        return errors;
    }
    private static ICollection<KeyValuePair<ValidationContext, object>> GetPropertyValues(object instance, ValidationContext validationContext)
    {
        var properties = instance.GetType().GetRuntimeProperties().Where(p => ValidationAttributeStore.IsPublic(p) && !p.GetIndexParameters().Any());
        // ReSharper disable once PossibleMultipleEnumeration
        var items = new List<KeyValuePair<ValidationContext, object>>(properties.Count());
        // ReSharper disable once PossibleMultipleEnumeration
        foreach (var property in properties)
        {
            var context = CreateValidationContext(instance, validationContext);
            context.MemberName = property.Name;
                
            //Supermodel Addition
            context.DisplayName = instance.GetType().GetDisplayNameForProperty(property.Name);
 
            if (_store.GetPropertyValidationAttributes(context).Any())
            {
                items.Add(new KeyValuePair<ValidationContext, object>(context, property.GetValue(instance, null)));
            }
        }
 
        return items;
    }
    private static IEnumerable<ValidationError> GetValidationErrors(object? value, ValidationContext validationContext, IEnumerable<ValidationAttribute> attributes, bool breakOnFirstError)
    {
        if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));

        var errors = new List<ValidationError>();
        ValidationError? validationError;

        // Get the required validator if there is one and test it first, aborting on failure
        // ReSharper disable once PossibleMultipleEnumeration
        var required = attributes.OfType<RequiredAttribute>().FirstOrDefault();
        if (required != null)
        {
            var valueToCheck = value;
            if (valueToCheck is IUIComponentWithValue componentWithValue) valueToCheck = componentWithValue.ComponentValue;

            if (!TryValidate(valueToCheck, validationContext, required, out validationError))
            {
                errors.Add(validationError!);
                return errors;
            }
        }

        // Iterate through the rest of the validators, skipping the required validator
        // ReSharper disable once PossibleMultipleEnumeration
        foreach (var attr in attributes)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (attr != required)
            {
                if (!TryValidate(value, validationContext, attr, out validationError))
                {
                    errors.Add(validationError!);
                    if (breakOnFirstError) break;
                }
            }
        }

        return errors;
    }
    private static bool TryValidate(object? value, ValidationContext validationContext, ValidationAttribute attribute, out ValidationError? validationError)
    {
        Debug.Assert(validationContext != null);

        var validationResult = attribute.GetValidationResult(value, validationContext);
        if (validationResult != ValidationResult.Success)
        {
            validationError = new ValidationError(attribute, value, validationResult!);
            return false;
        }

        validationError = null;
        return true;
    }
    #endregion

    #region Properties
    private static readonly ValidationAttributeStore _store = ValidationAttributeStore.Instance;
    #endregion
}