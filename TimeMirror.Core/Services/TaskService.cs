using System;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.Core.Services
{
    // Issue #47 - Vincent
    // Business logic for tasks: checks the data is valid,
    // then hands it to the repository to be saved.
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;

        // The repository comes in through dependency injection,
        // so this class never creates one itself.
        public TaskService(ITaskRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public TaskItem CreateTask(TaskItem task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Task title is required.", nameof(task));

            if (task.AgendaID <= 0)
                throw new ArgumentException("Task must belong to an agenda.", nameof(task));

            if (task.DueDate == default)
                throw new ArgumentException("Task must have a due date.", nameof(task));

            task.Title = task.Title.Trim();

            return _repository.Add(task);
        }
    }
}