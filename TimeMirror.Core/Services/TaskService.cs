using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public TaskItem? GetTaskById(int taskId)
        {
            if (taskId <= 0)
            {
                return null;
            }

            return _repository.FindById(taskId);
        }
    }
}