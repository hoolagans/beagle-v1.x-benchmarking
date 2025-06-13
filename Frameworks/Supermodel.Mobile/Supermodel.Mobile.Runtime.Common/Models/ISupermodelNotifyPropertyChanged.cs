using System.ComponentModel;

namespace Supermodel.Mobile.Runtime.Common.Models;

public interface ISupermodelNotifyPropertyChanged : INotifyPropertyChanged
{
    void OnPropertyChanged(string propertyName);
}