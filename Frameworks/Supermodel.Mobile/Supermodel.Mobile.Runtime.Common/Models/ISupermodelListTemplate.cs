using System;
using Xamarin.Forms;

namespace Supermodel.Mobile.Runtime.Common.Models;

public interface ISupermodelListTemplate : ISupermodelNotifyPropertyChanged
{
    DataTemplate GetListCellDataTemplate(EventHandler selectItemHandler, EventHandler deleteItemHandler);
}