using System;
using Supermodel.DataAnnotations.Validations;

namespace Supermodel.Presentation.Cmd.Models;

public static class CmdContext
{
    #region Embedded Types
    public struct RequiredState : IDisposable
    {
        #region Constructors
        internal RequiredState(bool newIsPropertyRequired, string? newPropertyDisplayName)
        {
            SavedPropertyIsRequired = CmdContext.IsPropertyRequired;
            CmdContext.IsPropertyRequired = newIsPropertyRequired;

            SavedPropertyDisplayName = CmdContext.PropertyDisplayName;
            CmdContext.PropertyDisplayName = newPropertyDisplayName;
        }
        #endregion


        #region Properties
        public bool SavedPropertyIsRequired { get; set; }
        public string? SavedPropertyDisplayName { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            CmdContext.IsPropertyRequired = SavedPropertyIsRequired;
            CmdContext.PropertyDisplayName = SavedPropertyDisplayName;
        }
        #endregion
    }
    #endregion

    #region Methods
    public static RequiredState NewRequiredScope(bool newIsRequired, string? newPropertyName)
    {
        return CurrentRequiredState = new RequiredState(newIsRequired, newPropertyName);
    }
    #endregion

    #region Properties
    public static ValidationResultList ValidationResultList { get; set; } = new();
        
    public static string? PropertyDisplayName { get; set; }
    public static bool IsPropertyRequired { get; set; }

    public static bool CtrlEscEnabled { get; set; }

    public static object? RootParent { get; set; }
        
    //public static ValidationResultList ValidationResultList 
    //{ 
    //    get 
    //    {
    //        _validationResultList.Value ??= new ValidationResultList();
    //        return _validationResultList.Value;
    //    }
    //}
    //private static readonly AsyncLocal<ValidationResultList> _validationResultList = new AsyncLocal<ValidationResultList> { Value = new ValidationResultList() };

    //public static string? PropertyDisplayName
    //{
    //    get => _propertyDisplayName.Value!;
    //    set => _propertyDisplayName.Value = value;
    //}
    //private static readonly AsyncLocal<string?> _propertyDisplayName = new AsyncLocal<string?>();

    //public static bool IsPropertyRequired
    //{
    //    get => _isPropertyRequired.Value!;
    //    set => _isPropertyRequired.Value = value;
    //}
    //private static readonly AsyncLocal<bool> _isPropertyRequired = new AsyncLocal<bool>();
        
    //public static bool CtrlEscEnabled
    //{
    //    get => _ctrlEscEnabled.Value!;
    //    set => _ctrlEscEnabled.Value = value;
    //}
    //private static readonly AsyncLocal<bool> _ctrlEscEnabled = new AsyncLocal<bool>();

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private static RequiredState CurrentRequiredState { get; set; } //this is to store it away from GC
    #endregion
}