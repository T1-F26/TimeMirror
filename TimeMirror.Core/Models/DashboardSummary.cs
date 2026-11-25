using System;
using System.Collections.Generic;

namespace TimeMirror.Core.Models;

public class DashboardSummary
{
    public List<TaskItem> TodaysTasks { get; set; } = new();

    public List<TaskItem> UpcomingTasks { get; set; } = new();

    public int CompletedTaskCount { get; set; }

    public int PendingTaskCount { get; set; }

    public List<ActivityRecord> RecentActivity { get; set; } = new();

    public string MostUsedApplication { get; set; } = "";

    public TimeSpan MostUsedApplicationDuration { get; set; }

    public TimeSpan TotalTrackedTime { get; set; }
}