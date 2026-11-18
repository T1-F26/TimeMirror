
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int taskId);

    Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId);

    Task<List<TaskItem>> GetAllAsync();

    TaskItem AddTask(int agendaId, TaskItem task, int userId);

    Task UpdateAsync(TaskItem task);

    Task DeleteAsync(int taskId);

    List<TaskItem> GetTasksByUserId(int userId);
}
