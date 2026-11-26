
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAgendaService _agendaService;
        private readonly ITaskService _taskService;
        private readonly IActivityTrackingService _activityTrackingService;

        public DashboardService(
            IAgendaService agendaService,
            ITaskService taskService,
            IActivityTrackingService activityTrackingService)
        {
            _agendaService = agendaService;
            _taskService = taskService;
            _activityTrackingService = activityTrackingService;
        }

        public async Task<DashboardSummary> GetDashboardAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            var now = DateTime.Now;
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            // Retrieve the user's agenda and tasks.
            var agenda = _agendaService.GetAgenda(userId);

            var tasks = new List<TaskItem>();

            if (agenda != null)
            {
                tasks = await _taskService
                    .GetTasksByAgendaIdAsync(agenda.AgendaID);
            }

            // Include all tasks scheduled for today,
            // including completed tasks.
            var todaysTasks = tasks
                .Where(t => t.DueDate.Date == today)
                .ToList();

            // Include today's tasks and pending future tasks.
            var scheduledTasks = tasks
                .Where(t =>
                    t.DueDate.Date == today ||
                    (t.DueDate.Date > today && !t.IsCompleted))
                .OrderBy(t => t.DueDate)
                .ToList();

            // Retrieve activity records.
            var activities = await _activityTrackingService
                .GetActivitiesAsync(userId);

            // Include activities overlapping today's date.
            var todaysActivities = activities
                .Where(a =>
                    a.StartTime < tomorrow &&
                    (a.EndTime ?? now) > today)
                .OrderByDescending(a => a.StartTime)
                .ToList();

            // Determine most used application by duration.
            var mostUsedApplication = todaysActivities
                .GroupBy(a => a.ApplicationName)
                .OrderByDescending(g => g.Sum(a =>
                    GetDurationToday(a, today, now).Ticks))
                .Select(g => g
                    .OrderByDescending(a => a.StartTime)
                    .First())
                .FirstOrDefault();

            return new DashboardSummary
            {
                UpcomingTasks = scheduledTasks,

                CompletedTaskCount = todaysTasks
                    .Count(t => t.IsCompleted),

                PendingTaskCount = todaysTasks
                    .Count(t => !t.IsCompleted),

                RecentActivity = todaysActivities,

                FrequentVisit = mostUsedApplication
            };
        }

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
    }
}
