using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{
    // Contract for saving/loading tasks.
    // Defined by Vincent for TaskService (#47).
    // Implemented by Dray in TaskRepository (#46).
    public interface ITaskRepository
    {
        // Saves a new task and returns it with its TaskID set.
        List<TaskItem> GetTasksByUserId(int userId);

        TaskItem AddTask(int agendaId, TaskItem task, int userId);
        //void UpdateTask(int userId, TaskItem task);
        //void DeleteTask(int userId, int taskId);
        //TaskItem GetTaskById(int userId, int taskId);

    }
}