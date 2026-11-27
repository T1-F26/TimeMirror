using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Enums;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories;

public class PermissionRepository : IPermissionRepository
{
    private readonly IDbContextFactory<TimeMirrorDbContext> _factory;

    public PermissionRepository(
        IDbContextFactory<TimeMirrorDbContext> factory)
    {
        _factory = factory;
    }

    // Get all permissions for a specific user
    public async Task<List<UserPermission>> GetByUserIdAsync(int userId)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        return await context.UserPermissions
            .AsNoTracking()
            .Where(p => p.UserID == userId)
            .ToListAsync();
    }

    // Get a specific permission for a user
    public async Task<UserPermission?> GetByTypeAsync(
        int userId, PermissionType permissionType)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        return await context.UserPermissions
            .AsNoTracking()
            .FirstOrDefaultAsync(p =>
                p.UserID == userId &&
                p.PermissionType == permissionType);
    }

    // Update an existing permission
    public async Task UpdateAsync(UserPermission permission)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        var existingPermission = await context.UserPermissions
            .FirstOrDefaultAsync(p =>
                p.UserID == permission.UserID &&
                p.PermissionType == permission.PermissionType);

        if (existingPermission == null)
            return;

        existingPermission.IsGranted = permission.IsGranted;

        await context.SaveChangesAsync();
    }
}