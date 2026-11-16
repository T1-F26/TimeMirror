
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories
{
    // Handles task data in the database
    public class TaskRepository : ITaskRepository
    {
        private readonly TimeMirrorDbContext _context;

        // Initialize database context
        public TaskRepository(TimeMirrorDbContext context)
        {
            _context = context;
        }

        // Get a task by its ID
        public async Task<TaskItem?> GetByIdAsync(int taskId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskID == taskId);
        }

        // Get all tasks belonging to an agenda
        public async Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId)
        {
            return await _context.Tasks
                .Where(t => t.AgendaID == agendaId)
                .ToListAsync();
        }

        // Get all tasks
        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        // Add a new task
        public async Task AddAsync(TaskItem task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        // Update an existing task
        public async Task UpdateAsync(TaskItem task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }

        // Delete a task by its ID
        public async Task DeleteAsync(int taskId)
        {
            var task = await GetByIdAsync(taskId);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
