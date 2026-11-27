using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Enums;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<List<UserPermission>> GetByUserIdAsync(int userId);

    Task<UserPermission?> GetByTypeAsync(
        int userId, PermissionType permissionType);

    Task UpdateAsync(UserPermission permission);
}