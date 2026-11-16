
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{
    // Defines methods for managing tasks in the database
    public interface ITaskRepository
    {
        // Get task by ID
        Task<TaskItem?> GetByIdAsync(int taskId);

        // Get all tasks belonging to an agenda
        Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId);

        // Get all tasks
        Task<List<TaskItem>> GetAllAsync();

        // Add a new task
        Task AddAsync(TaskItem task);

        // Update an existing task
        Task UpdateAsync(TaskItem task);

        // Delete task by ID
        Task DeleteAsync(int taskId);

        // Get tasks belonging to a user
        List<TaskItem> GetTasksByUserId(int userId);

        // Add a task to an agenda for a user
        TaskItem AddTask(int agendaId, TaskItem task, int userId);
    }
}
