using System.Collections.Generic;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.XForms.ViewModels;

public interface ISupermodelMobileDetailTemplate
{
    List<Cell> RenderDetail(Page parentPage, int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue);
}