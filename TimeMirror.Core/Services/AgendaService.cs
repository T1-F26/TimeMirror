
using System;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Services
{
    // Issue #58 - Braiden
    // This service retrieves a user's agenda and its tasks
    // using the agenda repository.
    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _repository;

        // The repository is passed in through dependency injection
        // so the service doesn't need to create one itself.
        public AgendaService(IAgendaRepository repository)
        {
            _repository = repository;
        }

        // Gets the agenda belonging to the requested user.
        public Agenda? GetAgenda(int userId)
        {
            // Check that the user ID is valid before searching.
            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(userId),
                    "User ID must be greater than zero.");
            }

            // The repository returns the agenda with its tasks.
            // If the user has no agenda, it returns null.
            return _repository.GetByUserId(userId);
        }

        // Agenda filtering and search are being handled separately
        // This service currently handles retrieving the agenda only.
    }
}
