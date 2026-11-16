using TimeMirror.Core.Models;
using TimeMirror.Core.Interfaces.Repositories;

namespace TimeMirror.Infrastructure.Data;

public class LocalDb
{
    public Dictionary<int, User> Users { get; } = new();

    public LocalDb()
    {
        var user = new User
        {
            UserID = 1,
            // Other required User properties
        };

        var agenda = new Agenda
        {
            AgendaID = 1,
            UserID = 1,
            Description = "My Agenda",
            Tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    TaskID = 1,
                    AgendaID = 1,
                    Title = "Team Meeting",
                    DueDate = new DateTime(2026, 11, 17, 10, 0, 0)
                },
                new TaskItem
                {
                    TaskID = 2,
                    AgendaID = 1,
                    Title = "COMP231 Project",
                    DueDate = new DateTime(2026, 11, 19, 14, 0, 0)
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

        // Assuming your User model has an Agenda property
        user.Agenda = agenda;

        Users.Add(user.UserID, user);
    }
}