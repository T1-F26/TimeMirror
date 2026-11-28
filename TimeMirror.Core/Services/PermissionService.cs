using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Services
{
    // Issue #104 - Implement permission service
    // User Story #99 - View permissions granted to TimeMirror
    //
    // Business layer between PermissionsViewModel (#105) and
    // PermissionRepository (#103). The service checks the request,
    // asks the repository for the user's saved permission records,
    // and returns them so the view can show Granted / Not granted.
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _repository;

        // The repository is passed in through dependency injection
        // so the service doesn't need to create one itself.
        public PermissionService(IPermissionRepository repository)
        {
            _repository = repository ??
                throw new ArgumentNullException(nameof(repository));
        }

        // Retrieves the permissions saved for the requested user.
        // Only that user's records are returned (the repository filters
        // by UserID). If the user has no permission records, an empty
        // list is returned so the view can show its empty state.
        public async Task<List<UserPermission>> GetPermissionsAsync(int userId)
        {
            // Check that the user ID is valid before searching.
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(userId),
                    "User ID must be greater than zero.");
            }

            var permissions = await _repository.GetByUserIdAsync(userId);

            // Never hand a null list to the ViewModel.
            if (permissions == null)
                return new List<UserPermission>();

            // Keep the display order the same every time the page loads.
            return permissions
                .OrderBy(p => p.PermissionType)
                .ToList();
        }
    }
}
