using System.Collections.Generic;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;


namespace TimeMirror.Infrastructure.Repositories
{

    public class AgendaRepository : IAgendaRepository
    {
        private readonly LocalDb _db;

        public AgendaRepository(LocalDb db)
        {
            _db = db;
        }
        public Agenda? GetByUserId(int userId)
        {
            return _db.Users.TryGetValue(userId, out var user)
                ? user.Agenda
                : null;
        }
    }
}