using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            _repository = repository ??
                throw new ArgumentNullException(nameof(repository));
        }

        // Issue #47 - Vincent
        // Creates a new task using the repository.
        public TaskItem CreateTask(TaskItem task, int agendaId, int userId)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Task title is required.");

            if (task.AgendaID <= 0)
                throw new ArgumentException("Task must belong to an agenda.");

            if (task.DueDate == default)
                throw new ArgumentException("Task must have a due date.");

            if (agendaId <= 0)
                throw new ArgumentException("Agenda ID is required.");

            if (string.IsNullOrWhiteSpace(task.Description))
                throw new ArgumentException("Task description is required.");

            if (userId <= 0)
                throw new ArgumentException("User ID is required.");

            task.Title = task.Title.Trim();

            return _repository.AddTask(agendaId, task, userId);
        }

        // Issue #39 - Vincent
        // Validates an edited task, checks it still exists, then saves it.
        public async Task UpdateTaskAsync(TaskItem task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            if (task.TaskID <= 0)
                throw new ArgumentException("Task ID is required.");

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Task title is required.");

            if (string.IsNullOrWhiteSpace(task.Description))
                throw new ArgumentException("Task description is required.");

            if (task.DueDate == default)
                throw new ArgumentException("Task must have a due date.");

            var existing = await _repository.GetByIdAsync(task.TaskID);

            if (existing == null)
                throw new ArgumentException("This task no longer exists.");

            // Keep the task in the same agenda it was created in.
            task.AgendaID = existing.AgendaID;
            task.Title = task.Title.Trim();

            await _repository.UpdateAsync(task);
        }

        // Issue #57
        // Retrieves a specific task from the repository.
        public async Task<TaskItem?> GetTaskByIdAsync(int taskId)
        {
            if (taskId <= 0)
                throw new ArgumentException("Task ID is required.");

            return await _repository.GetByIdAsync(taskId);
        }

        // Retrieves all tasks belonging to an agenda.
        public async Task<List<TaskItem>> GetTasksByAgendaIdAsync(int agendaId)
        {
            if (agendaId <= 0)
                throw new ArgumentException("Agenda ID is required.");

            return await _repository.GetByAgendaIdAsync(agendaId);
        }

        // Retrieves all tasks from the repository.
        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Retrieves all tasks belonging to a specific user.
        public List<TaskItem> GetTasksByUserId(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID is required.");

            return _repository.GetTasksByUserId(userId);
        }
    }
}