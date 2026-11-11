using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{
    // Contract for saving/loading tasks.
    // Defined by Vincent for TaskService (#47).
    // Implemented by Dray in TaskRepository (#46).
    public interface ITaskRepository
    {
        // Saves a new task and returns it with its TaskID set.
        TaskItem Add(TaskItem task);
    }
}