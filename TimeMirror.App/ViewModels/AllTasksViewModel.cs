using System;
using System.Collections.Generic;
using System.Linq;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// View Completed Tasks (#17) - Vincent
// #8:  organizes the user's tasks for the Tasks page and filters by Completed.
// #12: reuses the existing TaskService.GetTasksByUserId (validates the user,
//      then calls the task repository), so no new service code is needed.
// Clicking a task opens it in the same Task Details page as View/Edit Task.
public class AllTasksViewModel : ViewModelBase
{
    private readonly ITaskService _taskService;
    private readonly INavigationService _navigation;

    private List<TaskItem> _allTasks = new();
    private bool _showCompletedOnly;
    private string _errorMessage = "";
    private TaskItem? _selectedTask;

    // Services come in through dependency injection.
    public AllTasksViewModel(
        ITaskService taskService,
        UserSession userSession,
        INavigationService navigation)
    {
        _taskService = taskService;
        _navigation = navigation;

        LoadTasks(userSession.CurrentUserId);
    }

    // The list the page shows: every task, or only completed ones.
    public IEnumerable<TaskItem> VisibleTasks =>
        ShowCompletedOnly
            ? _allTasks.Where(t => t.IsCompleted)
            : _allTasks;

    // Bound to the "Show completed only" checkbox.
    public bool ShowCompletedOnly
    {
        get => _showCompletedOnly;
        set
        {
            _showCompletedOnly = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(VisibleTasks));
            OnPropertyChanged(nameof(EmptyMessage));
        }
    }

    // Friendly text when the list is empty.
    public string EmptyMessage =>
        VisibleTasks.Any()
            ? ""
            : ShowCompletedOnly
                ? "You have no completed tasks yet."
                : "You have no tasks yet.";

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    // Clicking a task opens it in Task Details (same as the Agenda).
    public TaskItem? SelectedTask
    {
        get => _selectedTask;
        set
        {
            _selectedTask = value;
            OnPropertyChanged();

            if (value != null)
                OpenTask(value.TaskID);
        }
    }

    private void LoadTasks(int userId)
    {
        try
        {
            _allTasks = _taskService.GetTasksByUserId(userId);
        }
        catch (Exception ex)
        {
            _allTasks = new List<TaskItem>();
            ErrorMessage = $"Could not load your tasks: {ex.Message}";
        }

        OnPropertyChanged(nameof(VisibleTasks));
        OnPropertyChanged(nameof(EmptyMessage));
    }

    private void OpenTask(int taskId)
    {
        _navigation.NavigateTo<TaskViewModel>();

        if (_navigation.CurrentViewModel is TaskViewModel taskViewModel)
            _ = taskViewModel.LoadTaskByIdAsync(taskId);
    }
}