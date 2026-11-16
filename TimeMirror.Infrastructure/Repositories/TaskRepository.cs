using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories
{
    public class TaskRepository: ITaskRepository
    {
        private readonly LocalDb _db;
        public TaskRepository(LocalDb db)
        {
            _db = db;
        }
        //CRUD operations

        //Get all tasks for a specific user
        public List<TaskItem> GetTasksByUserId(int userId)
        {
            if (_db.Users.TryGetValue(userId, out var user) && user.Agenda != null)
            {
                return user.Agenda.Tasks;
            }
            return new List<TaskItem>();
        }

        //Create a task
        public TaskItem AddTask(int agendaId, TaskItem task, int userId)
        {
            if (!_db.Users.TryGetValue(userId, out var user))
                throw new KeyNotFoundException("User not found.");

            if (user.Agenda == null)
                throw new InvalidOperationException("User has no agenda.");

            user.Agenda.Tasks.Add(task);
            return task;
        }

        // Read a task by its ID


        // Update a task


        // Delete a task
    }
}


