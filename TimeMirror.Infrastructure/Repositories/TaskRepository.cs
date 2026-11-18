
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeMirror.Core.Interfaces.Repositories;
using TimeMirror.Core.Models;
using TimeMirror.Infrastructure.Data;

namespace TimeMirror.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IDbContextFactory<TimeMirrorDbContext> _factory;

    public TaskRepository(
        IDbContextFactory<TimeMirrorDbContext> factory)
    {
        _factory = factory;
    }

    // READ: Get task by ID
    public async Task<TaskItem?> GetByIdAsync(int taskId)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        return await context.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TaskID == taskId);
    }

    // READ: Get all tasks belonging to an agenda
    public async Task<List<TaskItem>> GetByAgendaIdAsync(int agendaId)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        return await context.Tasks
            .AsNoTracking()
            .Where(t => t.AgendaID == agendaId)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    // READ: Get every task
    public async Task<List<TaskItem>> GetAllAsync()
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        return await context.Tasks
            .AsNoTracking()
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    // CREATE: Add a task to a user's agenda
    public TaskItem AddTask(
        int agendaId,
        TaskItem task,
        int userId)
    {
        using var context = _factory.CreateDbContext();

        bool agendaExists = context.Agendas.Any(a =>
            a.AgendaID == agendaId &&
            a.UserID == userId);

        if (!agendaExists)
            throw new InvalidOperationException(
                "Agenda not found or does not belong to this user.");

        task.AgendaID = agendaId;

        context.Tasks.Add(task);
        context.SaveChanges();

        return task;
    }

    // UPDATE: Modify an existing task
    public async Task UpdateAsync(TaskItem task)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        context.Tasks.Update(task);

        await context.SaveChangesAsync();
    }

    // DELETE: Remove a task by ID
    public async Task DeleteAsync(int taskId)
    {
        await using var context =
            await _factory.CreateDbContextAsync();

        var task = await context.Tasks
            .FirstOrDefaultAsync(t => t.TaskID == taskId);

        if (task == null)
            return;

        context.Tasks.Remove(task);

        await context.SaveChangesAsync();
    }

    // READ: Get all tasks belonging to a user
    public List<TaskItem> GetTasksByUserId(int userId)
    {
        using var context = _factory.CreateDbContext();

        return context.Tasks
            .AsNoTracking()
            .Where(t => context.Agendas.Any(a =>
                a.AgendaID == t.AgendaID &&
                a.UserID == userId))
            .OrderBy(t => t.DueDate)
            .ToList();
    }
}
