using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services
{
    // Issue #47 - Vincent
    // Contract for task business logic.
    // ViewModels depend on this interface, not on TaskService directly.
    public interface ITaskService
    {
        // Validates a new task and saves it through the task repository.
        // Returns the saved task (with its new TaskID).
        TaskItem CreateTask(TaskItem task);
    }
}