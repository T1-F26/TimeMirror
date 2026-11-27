using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services
{
    public interface IPermissionService
    {
        // Retrieves the permissions granted for a user
        Task<List<UserPermission>> GetPermissionsAsync(int userId);
    }
}