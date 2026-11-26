
using System;
using Microsoft.EntityFrameworkCore;
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

        // Read SQL Server connection string from environment
        string connectionString =
            Environment.GetEnvironmentVariable(
                "TIMEMIRROR_CONNECTION_STRING")
            ?? throw new InvalidOperationException(
                "TIMEMIRROR_CONNECTION_STRING is not configured.");

        var services = new ServiceCollection();

        // SQL Server / EF Core
        services.AddDbContextFactory<TimeMirrorDbContext>(
            options => options.UseSqlServer(connectionString));

        // Repositories
        services.AddSingleton<IAgendaRepository, AgendaRepository>();
        services.AddSingleton<ITaskRepository, TaskRepository>();
        services.AddSingleton<IActivityTrackingRepository, ActivityTrackingRepository>();

        // Current user
        services.AddSingleton<UserSession>();

        // Business services
        services.AddSingleton<IAgendaService, AgendaService>();
        services.AddSingleton<ITaskService, TaskService>();
        services.AddSingleton<IActivityTrackingService, ActivityTrackingService>();
        services.AddSingleton<IDashboardService, DashboardService>();
        // Navigation
        services.AddSingleton<INavigationService, NavigationService>();

        // ViewModels
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<AgendaViewModel>();
        services.AddTransient<TaskViewModel>();
        services.AddTransient<AllTasksViewModel>();
        services.AddTransient<ActivityTrackingViewModel>();
        services.AddTransient<DashboardViewModel>();
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
                throw new InvalidOperationException(
                    "No agenda was found for the current user.");

            return new NewTaskViewModel(
                taskService,
                userSession,
                agenda.AgendaID);
        });

        // Main window
        services.AddSingleton<MainWindow>();

        _provider = services.BuildServiceProvider();


        // UserSession - This is probably temporary as we dont have a login system yet. For now, we will just set the current user to 1.
        using (var context = _provider
    .GetRequiredService<
        IDbContextFactory<TimeMirrorDbContext>>()
    .CreateDbContext())
        {
            var user = context.Users
                .SingleOrDefault(u =>
                    u.Email == "test@timemirror.local");

            if (user == null)
                throw new InvalidOperationException(
                    "Test user not found. Run SeedTestData.sql.");

            var session = _provider
                .GetRequiredService<UserSession>();

            session.CurrentUserId = user.UserID;
        }
  
        var window =
            _provider.GetRequiredService<MainWindow>();

        var navigation =
            _provider.GetRequiredService<INavigationService>();

        navigation.NavigateTo<AgendaViewModel>();

        MainWindow = window;
        window.Show();
    }


    protected override async void OnExit(ExitEventArgs e)
    {
        try
        {
            if (_provider != null)
            {
                await _provider.DisposeAsync();
            }
        }
        finally
        {
            base.OnExit(e);
        }
    }

}
