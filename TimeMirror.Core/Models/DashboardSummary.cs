using System.Collections.Generic;

namespace TimeMirror.Core.Models
{
    public class DashboardSummary
    {
        // Tasks that are coming up
        public List<TaskItem> UpcomingTasks { get; set; } = new();

        // Number of completed tasks
        public int CompletedTaskCount { get; set; }

        // Number of tasks that have not been completed
        public int PendingTaskCount { get; set; }

        // Recently recorded activities
        public List<ActivityRecord> RecentActivity { get; set; } = new();

        // Most frequently visited application
        public ActivityRecord? FrequentVisit { get; set; }
    }
}