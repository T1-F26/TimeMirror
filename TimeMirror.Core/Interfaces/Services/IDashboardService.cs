using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services
{
    public interface IDashboardService
    {
        // Retrieves the user's dashboard information
        Task<DashboardSummary> GetDashboardAsync(int userId);
    }
}