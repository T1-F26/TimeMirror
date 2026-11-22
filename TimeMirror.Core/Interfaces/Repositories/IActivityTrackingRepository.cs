
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{
    // Methods for managing activity tracking records
    public interface IActivityTrackingRepository
    {
        // Get activity by ID
        Task<ActivityRecord?> GetByIdAsync(int activityId);

        // Get all activities for specific user
        Task<List<ActivityRecord>> GetByUserIdAsync(int userId);

        // Get activities within date range
        Task<List<ActivityRecord>> GetByDateRangeAsync(
            int userId,
            DateTime startDate,
            DateTime endDate);

        // Add new activity record
        Task AddAsync(ActivityRecord activity);

        // Update existing activity record
        Task UpdateAsync(ActivityRecord activity);
    }
}

