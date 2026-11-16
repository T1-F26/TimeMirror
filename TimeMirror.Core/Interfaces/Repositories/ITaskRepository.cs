
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{

    public interface ITaskRepository
    {
        // Get task by ID
        Task<TaskItem?> GetByIdAsync(int taskId);

        // Get all tasks belonging to agenda
        Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId);

        // Get all tasks
        Task<List<TaskItem>> GetAllAsync();

        // Add task
        Task AddAsync(TaskItem task);

        // Update existing task
        Task UpdateAsync(TaskItem task);

        // Delete task by ID
        Task DeleteAsync(int taskId);
    }
}
