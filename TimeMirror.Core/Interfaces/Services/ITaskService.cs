using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services
{
    // Issue #47 - Create task
    // Issue #57 - Retrieve task data
    public interface ITaskService
    {
        // Creates and saves a new task.
        TaskItem CreateTask(TaskItem task, int agendaId, int userId);

        // Retrieves a specific task by its ID.
        Task<TaskItem?> GetTaskByIdAsync(int taskId);

        // Retrieves all tasks belonging to an agenda.
        Task<List<TaskItem>> GetTasksByAgendaIdAsync(int agendaId);

        // Retrieves all tasks.
        Task<List<TaskItem>> GetAllTasksAsync();

        // Retrieves all tasks belonging to a user.
        List<TaskItem> GetTasksByUserId(int userId);
    }
}