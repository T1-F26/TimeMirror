using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TimeMirror.App.Services;
using TimeMirror.App.ViewModels;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Services;
using TimeMirror.Infrastructure.Repositories;

namespace TimeMirror.App;

public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddSingleton<IAgendaRepository, AgendaRepository>();
        services.AddSingleton<IAgendaService, AgendaService>();

        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<ShellViewModel>();
        services.AddTransient<AgendaViewModel>();

        services.AddSingleton<MainWindow>();

        _provider = services.BuildServiceProvider();

        var window = _provider.GetRequiredService<MainWindow>();
        var navigation = _provider.GetRequiredService<INavigationService>();

        navigation.NavigateTo<AgendaViewModel>();

        MainWindow = window;
        window.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _provider?.Dispose();
        base.OnExit(e);
    }
}