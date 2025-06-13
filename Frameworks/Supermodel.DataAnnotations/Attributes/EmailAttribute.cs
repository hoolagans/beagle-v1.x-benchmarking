using System.ComponentModel.DataAnnotations;

namespace Supermodel.DataAnnotations.Attributes;

public class EmailAttribute : RegularExpressionAttribute 
{
    public EmailAttribute(string errorMessage = "Must be a valid email address") : base( RegexHelper.EmailRegex ) 
    {
        ErrorMessage = errorMessage;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext) 
    {   
        if (value is IUIComponentWithValue valueWithValue) 
        {
            if (string.IsNullOrEmpty(valueWithValue.ComponentValue)) return ValidationResult.Success!;
            return base.IsValid(valueWithValue.ComponentValue, validationContext)!;
        }

        if (value == null || string.IsNullOrEmpty(value.ToString())) return ValidationResult.Success!;
        return base.IsValid(value.ToString(), validationContext)!;
    }
}