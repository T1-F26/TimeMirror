using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;

namespace TimeMirror.App.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;

    public ViewModelBase? CurrentViewModel =>
        _navigation.CurrentViewModel;

    public ICommand NavigateAgendaCommand { get; }

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;

        NavigateAgendaCommand = new RelayCommand(
            () => _navigation.NavigateTo<AgendaViewModel>());

        _navigation.PropertyChanged += (_, _) =>
            OnPropertyChanged(nameof(CurrentViewModel));
    }
}