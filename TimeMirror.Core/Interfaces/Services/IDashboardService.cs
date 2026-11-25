using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services;

public interface IDashboardService
{
    // Retrieve the dashboard summary for the current user.
    Task<DashboardSummary> GetDashboardAsync(int userId);
}