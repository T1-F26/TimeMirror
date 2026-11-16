using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Services
{
    public interface ITaskService
    {
        TaskItem? GetTaskById(int taskId);
    }
}