
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories
{
    // Handles activity tracking data in the database
    public class ActivityTrackingRepository : IActivityTrackingRepository
    {
        private readonly IDbContextFactory<TimeMirrorDbContext> _factory;

        // Initialize database context factory
        public ActivityTrackingRepository(
            IDbContextFactory<TimeMirrorDbContext> factory)
        {
            _factory = factory;
        }

        // Get an activity by its ID
        public async Task<ActivityRecord?> GetByIdAsync(int activityId)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.ActivityRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.ActivityID == activityId);
        }

        // Get all activities for a specific user
        public async Task<List<ActivityRecord>> GetByUserIdAsync(int userId)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.ActivityRecords
                .AsNoTracking()
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.StartTime)
                .ToListAsync();
        }

        // Get activities within a date range
        public async Task<List<ActivityRecord>> GetByDateRangeAsync(
            int userId, DateTime startDate, DateTime endDate)
        {
            await using var context = await _factory.CreateDbContextAsync();

            return await context.ActivityRecords
                .AsNoTracking()
                .Where(a =>
                    a.UserID == userId &&
                    a.StartTime < endDate &&
                    (a.EndTime == null ||
                     a.EndTime > startDate))
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        // Add a new activity record
        public async Task AddAsync(ActivityRecord activity)
        {
            await using var context = await _factory.CreateDbContextAsync();

            await context.ActivityRecords.AddAsync(activity);
            await context.SaveChangesAsync();
        }

        // Update an existing activity record
        public async Task UpdateAsync(ActivityRecord activity)
        {
            await using var context = await _factory.CreateDbContextAsync();

            context.ActivityRecords.Update(activity);
            await context.SaveChangesAsync();
        }
    }
}
