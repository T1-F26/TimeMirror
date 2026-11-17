using System;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TimeMirror.App.Services;
using TimeMirror.App.ViewModels;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Services;
using TimeMirror.Infrastructure.Repositories;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.App;

public partial class App : Application
{
    private ServiceProvider? _provider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        services.AddSingleton<LocalDb, LocalDb>();

        services.AddSingleton<IAgendaRepository, AgendaRepository>();
        services.AddSingleton<ITaskRepository, TaskRepository>();

        services.AddSingleton<UserSession>();

        services.AddSingleton<IAgendaService, AgendaService>();
        services.AddSingleton<ITaskService, TaskService>();

        services.AddSingleton<INavigationService, NavigationService>();

        services.AddSingleton<ShellViewModel>();
        services.AddTransient<AgendaViewModel>();

        // Create the New Task ViewModel using the current user's agenda.
        services.AddTransient<NewTaskViewModel>(provider =>
        {
            var taskService =
                provider.GetRequiredService<ITaskService>();

            var agendaService =
                provider.GetRequiredService<IAgendaService>();

            var userSession =
                provider.GetRequiredService<UserSession>();

            var agenda =
                agendaService.GetAgenda(userSession.CurrentUserId);

            if (agenda == null)
            {
                throw new InvalidOperationException(
                    "No agenda was found for the current user.");
            }

            return new NewTaskViewModel(
                taskService,
                userSession,
                agenda.AgendaID);
        });

        services.AddSingleton<MainWindow>();

        _provider = services.BuildServiceProvider();

        var window = _provider.GetRequiredService<MainWindow>();
        var navigation =
            _provider.GetRequiredService<INavigationService>();

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