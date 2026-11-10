using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{


    public interface IAgendaRepository
    {
        Agenda? GetByUserId(int userId);
    }
}