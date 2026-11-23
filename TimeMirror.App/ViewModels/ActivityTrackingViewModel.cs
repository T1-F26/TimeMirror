
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// Holds activity information for the tracking page.
public sealed class ActivityTrackingViewModel : ViewModelBase
{
    private readonly IActivityTrackingService _service;
    private readonly int _userId;

    private string _errorMessage = "";
    private bool _isBusy;
    private bool _isTracking;

    public ObservableCollection<ActivityRecord> Activities { get; } = new();

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public bool IsTracking
    {
        get => _isTracking;
        private set
        {
            _isTracking = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TrackingStatus));
        }
    }

    public string TrackingStatus =>
        IsTracking ? "Tracking active window" : "Tracking stopped";

    public int SessionCount => Activities.Count;

    // Adds together the durations of completed activities.
    public TimeSpan TotalRecordedTime => TimeSpan.FromTicks(
        Activities
            .Where(a => a.EndTime.HasValue)
            .Sum(a => Math.Max(
                0,
                (a.EndTime!.Value - a.StartTime).Ticks)));

    public ICommand RefreshCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    public ActivityTrackingViewModel(
        IActivityTrackingService service,
        UserSession session)
    {
        _service = service ??
            throw new ArgumentNullException(nameof(service));

        if (session == null)
            throw new ArgumentNullException(nameof(session));

        _userId = session.CurrentUserId;

        IsTracking = _service.IsTracking;

        // Commands used by the tracking page buttons.
        RefreshCommand = new RelayCommand(() => _ = RefreshAsync());
        StartCommand = new RelayCommand(() => _ = StartAsync());
        StopCommand = new RelayCommand(() => _ = StopAsync());
    }

    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var records = await _service.GetActivitiesAsync(_userId);

            Activities.Clear();

            foreach (var record in records.OrderByDescending(a => a.StartTime))
            {
                Activities.Add(record);
            }

            OnPropertyChanged(nameof(SessionCount));
            OnPropertyChanged(nameof(TotalRecordedTime));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load activities: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task StartAsync()
    {
        if (IsBusy || IsTracking)
            return;

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            // Start recording the active application and window.
            await _service.StartTrackingAsync(_userId);

            IsTracking = _service.IsTracking;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not start tracking: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task StopAsync()
    {
        if (IsBusy || !IsTracking)
            return;

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            // Stop tracking and save the final activity.
            await _service.StopTrackingAsync();

            IsTracking = _service.IsTracking;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not stop tracking: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }

        // Reload activity history after stopping.
        await RefreshAsync();
    }
}
