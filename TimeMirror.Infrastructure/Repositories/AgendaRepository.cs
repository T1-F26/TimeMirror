using System.Collections.Generic;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;


namespace TimeMirror.Infrastructure.Repositories
{

    public class AgendaRepository : IAgendaRepository
    {
        public Agenda? GetByUserId(int userId)
        {
            if (userId != 1)
                return null;

            return new Agenda
            {
                AgendaID = 1,
                UserID = 1,
                Description = "My Agenda",

                Tasks = new List<TaskItem>
            {
                new TaskItem()
                {
                    TaskID = 1,
                    AgendaID = 1,
                    Title = "Team Meeting",
                    DueDate = DateTime.Today.AddHours(15)
                },
                new TaskItem()
                {
                    TaskID = 2,
                    AgendaID = 1,
                    Title = "COMP231 Project",
                    DueDate = DateTime.Today.AddDays(2).AddHours(12)
                },
                new TaskItem()
                {
                    TaskID = 3,
                    AgendaID = 1,
                    Title = "Review Assignments",
                    DueDate = DateTime.Today.AddDays(10).AddHours(18)
                }
            }
            };
        }
    }
}