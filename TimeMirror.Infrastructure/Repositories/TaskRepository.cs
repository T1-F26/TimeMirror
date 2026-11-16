
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories
{
    // Handles task data in the database
    public class TaskRepository : ITaskRepository
    {
        private readonly TimeMirrorDbContext? _context;
        private readonly LocalDb? _db;

        // Initialize SQL database context
        public TaskRepository(TimeMirrorDbContext context)
        {
            _context = context;
        }

        // Initialize local database
        public TaskRepository(LocalDb db)
        {
            _db = db;
        }

        // Get task by ID from SQL
        public async Task<TaskItem?> GetByIdAsync(int taskId)
        {
            return await GetSqlContext().Tasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId);
        }

        // Get tasks belonging to an agenda from SQL
        public async Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId)
        {
            return await GetSqlContext().Tasks
                .Where(t => t.AgendaID == agendaId)
                .ToListAsync();
        }

        // Get all tasks from SQL
        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await GetSqlContext().Tasks.ToListAsync();
        }

        // Add a task to SQL
        public async Task AddAsync(TaskItem task)
        {
            var context = GetSqlContext();
            await context.Tasks.AddAsync(task);
            await context.SaveChangesAsync();
        }

        // Update a task in SQL
        public async Task UpdateAsync(TaskItem task)
        {
            var context = GetSqlContext();
            context.Tasks.Update(task);
            await context.SaveChangesAsync();
        }

        // Delete a task from SQL
        public async Task DeleteAsync(int taskId)
        {
            var context = GetSqlContext();
            var task = await context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId);

            if (task != null)
            {
                context.Tasks.Remove(task);
                await context.SaveChangesAsync();
            }
        }

        // Get tasks belonging to a user from LocalDb
        public List<TaskItem> GetTasksByUserId(int userId)
        {
            var db = GetLocalDb();

            if (db.Users.TryGetValue(userId, out var user)
                && user.Agenda != null)
            {
                return user.Agenda.Tasks;
            }

            return new List<TaskItem>();
        }

        // Add a task to a user's agenda in LocalDb
        public TaskItem AddTask(int agendaId, TaskItem task, int userId)
        {
            var db = GetLocalDb();

            if (!db.Users.TryGetValue(userId, out var user))
                throw new KeyNotFoundException("User not found.");

            if (user.Agenda == null)
                throw new InvalidOperationException("User has no agenda.");

            user.Agenda.Tasks.Add(task);
            return task;
        }

        // Get the SQL context when available
        private TimeMirrorDbContext GetSqlContext()
        {
            return _context ?? throw new InvalidOperationException(
                "TaskRepository was not initialized with TimeMirrorDbContext.");
        }

        // Get the local database when available
        private LocalDb GetLocalDb()
        {
            return _db ?? throw new InvalidOperationException(
                "TaskRepository was not initialized with LocalDb.");
        }
    }
}
