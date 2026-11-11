using System.ComponentModel;
using TimeMirror.App.ViewModels;

namespace TimeMirror.App.Services;

public interface INavigationService : INotifyPropertyChanged
{
    ViewModelBase? CurrentViewModel { get; }
    void NavigateTo<T>() where T : ViewModelBase;
}