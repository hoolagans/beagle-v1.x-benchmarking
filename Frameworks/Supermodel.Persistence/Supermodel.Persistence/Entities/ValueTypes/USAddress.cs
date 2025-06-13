using System.ComponentModel.DataAnnotations;

namespace Supermodel.Persistence.Entities.ValueTypes;

public class USAddress : ValueObject
{
    #region Properties
    [MaxLength(100)] public string Street { get; set; } = "";
    [MaxLength(100)] public string City { get; set; } = "";
    [MaxLength(100)] public string State { get; set; } = "";
    [MaxLength(100)] public string Zip { get; set; } = "";
    #endregion
}