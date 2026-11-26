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
            // Make sure the user ID is valid
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            // Get the agenda belonging to the user
            var agenda = _agendaService.GetAgenda(userId);

            var tasks = new List<TaskItem>();

            // Get the tasks if the user has an agenda
            if (agenda != null)
            {
                tasks = await _taskService
                    .GetTasksByAgendaIdAsync(agenda.AgendaID);
            }

            // Get the user's recorded activities
            var activities = await _activityTrackingService
                .GetActivitiesAsync(userId);

            var now = DateTime.Now;

            // Organize the retrieved data for the dashboard
            return new DashboardSummary
            {
                UpcomingTasks = tasks
                    .Where(t => !t.IsCompleted && t.DueDate >= now)
                    .OrderBy(t => t.DueDate)
                    .ToList(),

                CompletedTaskCount = tasks
                    .Count(t => t.IsCompleted),

                PendingTaskCount = tasks
                    .Count(t => !t.IsCompleted),

                RecentActivity = activities
                    .OrderByDescending(a => a.StartTime)
                    .ToList(),

                FrequentVisit = activities
                    .GroupBy(a => a.ApplicationName)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g
                        .OrderByDescending(a => a.StartTime)
                        .First())
                    .FirstOrDefault()
            };
        }
    }
}