using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Services
{

    public class AgendaService : IAgendaService
    {
        private readonly IAgendaRepository _repository;

        public AgendaService(IAgendaRepository repository)
        {
            _repository = repository;
        }

        public Agenda? GetAgenda(int userId)
        {
            return _repository.GetByUserId(userId);
        }
    }
}