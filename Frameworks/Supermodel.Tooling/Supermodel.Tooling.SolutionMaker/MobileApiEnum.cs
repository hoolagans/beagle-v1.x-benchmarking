using System.ComponentModel;

namespace Supermodel.Tooling.SolutionMaker;

public enum MobileApiEnum 
{
    NoMobile,
    [Description("Platform's Native API")] Native,
    [Description("Xamarin.Forms")] XamarinForms,
}