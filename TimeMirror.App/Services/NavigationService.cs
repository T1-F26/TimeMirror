using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using TimeMirror.App.ViewModels;

namespace TimeMirror.App.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _provider;

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public ViewModelBase? CurrentViewModel { get; private set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NavigateTo<T>() where T : ViewModelBase
    {
        CurrentViewModel = _provider.GetRequiredService<T>();

        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(nameof(CurrentViewModel)));
    }
}