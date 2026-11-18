
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories;

public class AgendaRepository : IAgendaRepository
{
    private readonly IDbContextFactory<TimeMirrorDbContext> _contextFactory;

    public AgendaRepository(IDbContextFactory<TimeMirrorDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public Agenda? GetByUserId(int userId)
    {
        using var context = _contextFactory.CreateDbContext();
        return context.Agendas
            .Include(a => a.Tasks)
            .FirstOrDefault(a => a.UserID == userId);
    }
}
