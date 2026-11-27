using System.Windows.Input;
using System.Windows.Navigation;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;

namespace TimeMirror.App.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private readonly INavigationService _navigation;
    public ViewModelBase? CurrentViewModel =>
        _navigation.CurrentViewModel;

    public ICommand NavigateAgendaCommand { get; }
    // Issue #13 - Vincent: sidebar button for the Tasks page (#17).
    public ICommand NavigateTasksCommand { get; }
    public ICommand ShowActivityTrackingCommand { get; }
    public ICommand ShowDashboardCommand { get; }
    public ICommand ShowPermissionsCommand { get; }

    public ShellViewModel(INavigationService navigation)
    {
        _navigation = navigation;
     

        NavigateAgendaCommand = new RelayCommand(
            () => _navigation.NavigateTo<AgendaViewModel>());

        NavigateTasksCommand = new RelayCommand(
            () => _navigation.NavigateTo<AllTasksViewModel>());

        _navigation.PropertyChanged += (_, _) =>
            OnPropertyChanged(nameof(CurrentViewModel));

        ShowActivityTrackingCommand = new RelayCommand(
    () => _navigation.NavigateTo<ActivityTrackingViewModel>());

        ShowDashboardCommand = new RelayCommand(
            () => _navigation.NavigateTo<DashboardViewModel>());

        ShowPermissionsCommand = new RelayCommand(
    () => _navigation.NavigateTo<PermissionsViewModel>());
    }
}