using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces
{
    public interface IAgendaService
    {
        Agenda? GetAgenda(int userId);

        IEnumerable<Task> GetUpcomingTasks(int userId, DateTime date, AgendaViewMode mode);
    }
}
