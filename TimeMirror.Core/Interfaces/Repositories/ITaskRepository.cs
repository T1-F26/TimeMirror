using TimeMirror.Core.Models;

namespace TimeMirror.Core.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        TaskItem? FindById(int taskId);
    }
}