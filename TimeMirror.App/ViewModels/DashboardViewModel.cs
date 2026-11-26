using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;
using System.Linq;


namespace TimeMirror.App.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    private readonly IDashboardService _dashboardService;
    private readonly UserSession _userSession;

    private int _completedTaskCount;
    private int _pendingTaskCount;
    private string _mostUsedApplication = "";
    private TimeSpan _mostUsedApplicationDuration;
    private TimeSpan _totalTrackedTime;
    private bool _isBusy;
    private string _errorMessage = "";

    private static TimeSpan GetDurationToday(
    ActivityRecord activity,
    DateTime today,
    DateTime now)
    {
        var start = activity.StartTime < today
            ? today
            : activity.StartTime;

        var end = activity.EndTime ?? now;

        if (end > now)
            end = now;

        return end > start
            ? end - start
            : TimeSpan.Zero;
    }

    public ObservableCollection<TaskItem> TodaysTasks { get; } = new();

    public ObservableCollection<TaskItem> UpcomingTasks { get; } = new();

    public ObservableCollection<ActivityRecord> RecentActivity { get; } = new();
    public ICommand LoadDashboardCommand { get; }
    public ICommand LoadRecentActivityCommand { get; }

    public int CompletedTaskCount
    {
        get => _completedTaskCount;
        private set
        {
            _completedTaskCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalTaskCount));
            OnPropertyChanged(nameof(TaskCompletionPercentage));
        }
    }

    public int PendingTaskCount
    {
        get => _pendingTaskCount;
        private set
        {
            _pendingTaskCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalTaskCount));
            OnPropertyChanged(nameof(TaskCompletionPercentage));
        }
    }

    public int TotalTaskCount =>
        CompletedTaskCount + PendingTaskCount;

    public double TaskCompletionPercentage =>
        TotalTaskCount == 0
            ? 0
            : (double)CompletedTaskCount / TotalTaskCount * 100;

    public string MostUsedApplication
    {
        get => _mostUsedApplication;
        private set
        {
            _mostUsedApplication = value;
            OnPropertyChanged();
        }
    }

    public TimeSpan MostUsedApplicationDuration
    {
        get => _mostUsedApplicationDuration;
        private set
        {
            _mostUsedApplicationDuration = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MostUsedApplicationDurationText));
            OnPropertyChanged(nameof(MostUsedApplicationPercentage));
        }
    }

    public TimeSpan TotalTrackedTime
    {
        get => _totalTrackedTime;
        private set
        {
            _totalTrackedTime = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalTrackedTimeText));
            OnPropertyChanged(nameof(MostUsedApplicationPercentage));
        }
    }

    public string TotalTrackedTimeText =>
        FormatDuration(TotalTrackedTime);

    public string MostUsedApplicationDurationText =>
        FormatDuration(MostUsedApplicationDuration);

    public double MostUsedApplicationPercentage =>
        TotalTrackedTime.TotalSeconds <= 0
            ? 0
            : MostUsedApplicationDuration.TotalSeconds
                / TotalTrackedTime.TotalSeconds * 100;

    public string CurrentDate =>
        DateTime.Today.ToString("dddd, MMMM d, yyyy");

    public bool HasTodaysTasks => TodaysTasks.Count > 0;

    public bool HasUpcomingTasks => UpcomingTasks.Count > 0;

    public bool HasRecentActivity => RecentActivity.Count > 0;

    public bool HasScheduledTasks =>
        HasTodaysTasks || HasUpcomingTasks;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    // Initialize dashboard dependencies and commands.
    public DashboardViewModel(
        IDashboardService dashboardService,
        UserSession userSession)
    {
        _dashboardService = dashboardService;
        _userSession = userSession;

        LoadDashboardCommand = new RelayCommand(
            () => _ = LoadAsync());

        LoadRecentActivityCommand = new RelayCommand(
            () => _ = LoadAsync());

        TodaysTasks.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasTodaysTasks));
            OnPropertyChanged(nameof(HasScheduledTasks));
        };

        UpcomingTasks.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasUpcomingTasks));
            OnPropertyChanged(nameof(HasScheduledTasks));
        };

        RecentActivity.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(HasRecentActivity));
        };
    }

    // Load dashboard information for the current user.

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            int userId = _userSession.CurrentUserId;

            var summary = await _dashboardService
                .GetDashboardAsync(userId);

            var now = DateTime.Now;
            var today = now.Date;

            // Today's tasks
            TodaysTasks.Clear();

            foreach (var task in summary.UpcomingTasks
                .Where(t => t.DueDate.Date == today))
            {
                TodaysTasks.Add(task);
            }

            // Upcoming tasks
            UpcomingTasks.Clear();

            foreach (var task in summary.UpcomingTasks
                .Where(t => t.DueDate.Date > today))
            {
                UpcomingTasks.Add(task);
            }

            // Daily task statistics
            CompletedTaskCount = summary.CompletedTaskCount;
            PendingTaskCount = summary.PendingTaskCount;

            // Recent activity: display latest 10 records
            RecentActivity.Clear();

            foreach (var activity in summary.RecentActivity
                .OrderByDescending(a => a.StartTime)
                .Take(10))
            {
                RecentActivity.Add(activity);
            }

            // Total tracked time today
            TotalTrackedTime = TimeSpan.FromTicks(
                summary.RecentActivity.Sum(a =>
                    GetDurationToday(a, today, now).Ticks));

            // Most used application today
            MostUsedApplication =
                summary.FrequentVisit?.ApplicationName
                ?? "No activity recorded";

            MostUsedApplicationDuration =
                summary.FrequentVisit == null
                    ? TimeSpan.Zero
                    : TimeSpan.FromTicks(
                        summary.RecentActivity
                            .Where(a => a.ApplicationName ==
                                summary.FrequentVisit.ApplicationName)
                            .Sum(a => GetDurationToday(
                                a, today, now).Ticks));

            OnPropertyChanged(nameof(CurrentDate));
        }
        catch (Exception ex)
        {
            ErrorMessage =
                $"Could not load dashboard: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Format tracked time for dashboard display.
    private static string FormatDuration(TimeSpan duration)
    {
        return $"{(int)duration.TotalHours}h {duration.Minutes}m";
    }
}