
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.Services;

// Tracks the application and window currently being used.
public sealed class ActivityTrackingService : IActivityTrackingService, IAsyncDisposable
{
    private readonly IActivityTrackingRepository _repository;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(2);
    private readonly SemaphoreSlim _stateLock = new(1, 1);

    private CancellationTokenSource? _cancel;
    private Task? _trackingTask;
    private ActivityRecord? _currentActivity;
    private int _userId;

    public bool IsTracking => _trackingTask is { IsCompleted: false };

    public ActivityTrackingService(IActivityTrackingRepository repository)
    {
        _repository = repository ??
            throw new ArgumentNullException(nameof(repository));
    }

    public async Task StartTrackingAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));

        await _stateLock.WaitAsync();

        try
        {
            if (IsTracking)
                return;

            _userId = userId;

            _cancel?.Dispose();
            _cancel = new CancellationTokenSource();

            _trackingTask = TrackDesktopAsync(_cancel.Token);
        }
        finally
        {
            _stateLock.Release();
        }
    }

    public async Task StopTrackingAsync()
    {
        Task? runningTask;

        await _stateLock.WaitAsync();

        try
        {
            _cancel?.Cancel();
            runningTask = _trackingTask;
        }
        finally
        {
            _stateLock.Release();
        }

        // Wait for the final activity to be saved.
        if (runningTask != null)
            await runningTask;
    }

    public Task<List<ActivityRecord>> GetActivitiesAsync(int userId)
    {
        ValidateUser(userId);

        return _repository.GetByUserIdAsync(userId);
    }

    public Task<List<ActivityRecord>> GetActivitiesByDateAsync(
        int userId,
        DateTime start,
        DateTime end)
    {
        ValidateUser(userId);

        if (end <= start)
            throw new ArgumentException("End must be after start.");

        return _repository.GetByDateRangeAsync(userId, start, end);
    }

    private async Task TrackDesktopAsync(CancellationToken token)
    {
        try
        {
            // Check the active window until tracking is stopped.
            while (!token.IsCancellationRequested)
            {
                string? activeWindow = GetActiveWindowInfo();

                await UpdateCurrentActivityAsync(activeWindow);

                await Task.Delay(_checkInterval, token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Normal when tracking is stopped.
        }
        finally
        {
            await EndCurrentActivityAsync();
        }
    }

    private async Task UpdateCurrentActivityAsync(string? windowInfo)
    {
        // Don't create another record if the window hasn't changed.
        if (string.Equals(
            _currentActivity?.ApplicationName,
            windowInfo,
            StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // Finish the previous activity before starting another.
        await EndCurrentActivityAsync();

        if (string.IsNullOrWhiteSpace(windowInfo))
            return;

        var activity = new ActivityRecord
        {
            UserID = _userId,
            ApplicationName = windowInfo,
            StartTime = DateTime.Now
        };

        await _repository.AddAsync(activity);

        _currentActivity = activity;
    }

    private async Task EndCurrentActivityAsync()
    {
        if (_currentActivity == null)
            return;

        var completed = _currentActivity;

        completed.EndTime = DateTime.Now;

        await _repository.UpdateAsync(completed);

        _currentActivity = null;
    }

    private static string? GetActiveWindowInfo()
    {
        // Get the window that currently has focus.
        IntPtr window = GetForegroundWindow();

        if (window == IntPtr.Zero)
            return null;

        GetWindowThreadProcessId(window, out uint processId);

        if (processId == 0)
            return null;

        try
        {
            using var process = Process.GetProcessById((int)processId);

            string applicationName = process.ProcessName;

            // Get the title displayed at the top of the window.
            int length = GetWindowTextLength(window);
            string windowTitle = "";

            if (length > 0)
            {
                var titleBuilder = new StringBuilder(length + 1);

                GetWindowText(
                    window,
                    titleBuilder,
                    titleBuilder.Capacity);

                windowTitle = titleBuilder.ToString().Trim();
            }

            if (string.IsNullOrWhiteSpace(windowTitle))
                return applicationName;

            // Keep the title reasonably short for storage.
            if (windowTitle.Length > 200)
                windowTitle = windowTitle.Substring(0, 200);

            return $"{applicationName} - {windowTitle}";
        }
        catch (ArgumentException)
        {
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }

    private static void ValidateUser(int userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException(nameof(userId));
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(
        IntPtr window,
        out uint processId);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(
        IntPtr window,
        StringBuilder text,
        int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr window);

    public async ValueTask DisposeAsync()
    {
        await StopTrackingAsync();

        _cancel?.Dispose();
        _stateLock.Dispose();
    }
}
