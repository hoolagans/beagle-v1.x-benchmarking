using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Supermodel.DataAnnotations.Exceptions;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public abstract class ComparisonAttribute : ValidationAttribute
{
    #region Constructors
    protected ComparisonAttribute(string match, string errorMessage)
    {
        MatchProperty = match;
        ErrorMessage = errorMessage;
    }
    #endregion

    #region Methods
    protected int GetComparisonResult(object? value, ValidationContext validationContext)
    {
        var objectType = validationContext.ObjectType;
        object matchValue;

        var linqMatches = objectType.GetProperties().Where(propertyInfo => propertyInfo.Name == MatchProperty).ToList();
        if (linqMatches.Any())
        {
            var propertyInfo = linqMatches.First();
            matchValue = propertyInfo.GetValue(validationContext.ObjectInstance, null);
        }
        else
        {
            throw new SupermodelException("Unable to find match to compare with");
        }

        var comparableValue = value as IComparable;
        var comparableMatchValue = matchValue as IComparable;
        if (comparableValue == null || comparableMatchValue == null) throw new SupermodelException("Property must implement IComparable in order to use comparison attributes");
        return comparableValue.CompareTo(comparableMatchValue);
    }
    #endregion

    #region Properties
    public string MatchProperty { get; set; }
    #endregion
}