using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// View Task - Maryam (#54), Vincent (#55)
// Backs the reusable TaskCardUC (#53) in 'ViewMode': the card's fields are
// read-only and are filled with the task's persisted data.
// #55: the task is retrieved by ID through ITaskService (LoadTaskByIdAsync).
public class TaskViewModel : ViewModelBase
{
    private readonly ITaskService _taskService;
    private readonly INavigationService _navigation;

    private int _taskId;
    private string _title = "";
    private string _description = "";
    private DateTime? _dueDate;
    private bool _isViewMode = true;
    private string _errorMessage = "";

    // Services come in through dependency injection.
    public TaskViewModel(ITaskService taskService, INavigationService navigation)
    {
        _taskService = taskService;
        _navigation = navigation;

        BackCommand = new RelayCommand(
            () => _navigation.NavigateTo<AgendaViewModel>());
    }

    public int TaskId
    {
        get => _taskId;
        private set { _taskId = value; OnPropertyChanged(); }
    }

    // TaskCardUC binds to these three properties (same names as NewTaskViewModel).
    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    // Nullable so it works with a WPF DatePicker.
    public DateTime? DueDate
    {
        get => _dueDate;
        set { _dueDate = value; OnPropertyChanged(); }
    }

    // TaskCardUC makes its fields read-only/disabled when this is true.
    // Starts in ViewMode; a setter (not get-only) so edit mode can switch it later.
    public bool IsViewMode
    {
        get => _isViewMode;
        set { _isViewMode = value; OnPropertyChanged(); }
    }

    // Shown on the page if the task can't be loaded.
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    // Returns to the Agenda.
    public ICommand BackCommand { get; }

    // Issue #55 - Vincent
    // Retrieves the persisted task through the task service,
    // then fills the card with it.
    public async Task LoadTaskByIdAsync(int taskId)
    {
        ErrorMessage = "";

        try
        {
            var task = await _taskService.GetTaskByIdAsync(taskId);

            if (task == null)
            {
                ErrorMessage = "This task could not be found.";
                return;
            }

            LoadTask(task);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load the task: {ex.Message}";
        }
    }

    // Fills the card's empty fields with the persisted task and
    // puts the card into ViewMode.
    public void LoadTask(TaskItem task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        TaskId = task.TaskID;
        Title = task.Title;
        Description = task.Description;
        DueDate = task.DueDate;
        IsViewMode = true;
    }
}