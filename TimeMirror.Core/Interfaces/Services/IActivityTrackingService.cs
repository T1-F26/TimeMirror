
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services;

// Methods used to control tracking and retrieve activity records.
public interface IActivityTrackingService
{
    bool IsTracking { get; }

    Task StartTrackingAsync(int userId);

    Task StopTrackingAsync();

    Task<List<ActivityRecord>> GetActivitiesAsync(int userId);

    Task<List<ActivityRecord>> GetActivitiesByDateAsync(
        int userId,
        DateTime start,
        DateTime end);
}
