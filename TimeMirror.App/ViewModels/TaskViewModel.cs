using System;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// View Task - Maryam (#54), Vincent (#55)
// Edit Task - Maryam (#35)
// Backs the reusable TaskCardUC (#53). The card starts in ViewMode (read-only).
// #55: the task is retrieved by ID through ITaskService (LoadTaskByIdAsync).
// #35: Edit switches the card to edit mode, Save sends the changes to
//      ITaskService.UpdateTaskAsync (#39), Cancel discards them.
public class TaskViewModel : ViewModelBase
{
    private readonly ITaskService _taskService;
    private readonly INavigationService _navigation;

    private int _taskId;
    private int _agendaId;
    private string _title = "";
    private string _description = "";
    private DateTime? _dueDate;
    private bool _isViewMode = true;
    private string _errorMessage = "";
    private string _successMessage = "";
    private bool _isSaving;

    // Copy of the last saved/loaded task, used by Cancel to undo unsaved edits
    // and to keep the original time of day when the date is changed.
    private TaskItem? _originalTask;

    // Services come in through dependency injection.
    public TaskViewModel(ITaskService taskService, INavigationService navigation)
    {
        _taskService = taskService;
        _navigation = navigation;

        BackCommand = new RelayCommand(
            () => _navigation.NavigateTo<AgendaViewModel>());

        // Issue #35
        EditCommand = new RelayCommand(EnterEditMode);
        SaveCommand = new RelayCommand(() => _ = SaveAsync());
        CancelCommand = new RelayCommand(CancelEdit);
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
    // Starts in ViewMode; Edit switches it to false, Save/Cancel switch it back.
    public bool IsViewMode
    {
        get => _isViewMode;
        set
        {
            _isViewMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsEditMode));
        }
    }

    // Handy for showing Save/Cancel only while editing.
    public bool IsEditMode => !IsViewMode;

    // Shown on the page if the task can't be loaded or saved.
    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    // Shown on the page after the changes were saved.
    public string SuccessMessage
    {
        get => _successMessage;
        set { _successMessage = value; OnPropertyChanged(); }
    }

    // Returns to the Agenda.
    public ICommand BackCommand { get; }

    // Issue #35 - Maryam
    public ICommand EditCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

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

    // Fills the card's fields with the persisted task and
    // puts the card into ViewMode.
    public void LoadTask(TaskItem task)
    {
        if (task == null)
            throw new ArgumentNullException(nameof(task));

        // Keep a copy so Cancel can restore it.
        _originalTask = new TaskItem
        {
            TaskID = task.TaskID,
            AgendaID = task.AgendaID,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate
        };

        _agendaId = task.AgendaID;

        TaskId = task.TaskID;
        Title = task.Title;
        Description = task.Description;
        DueDate = task.DueDate;
        IsViewMode = true;
    }

    // Issue #35 - Maryam
    // Unlocks the card's fields so the user can change them.
    private void EnterEditMode()
    {
        // Nothing to edit until a task has been loaded.
        if (_originalTask == null)
            return;

        ErrorMessage = "";
        SuccessMessage = "";
        IsViewMode = false;
    }

    // Issue #35 - Maryam
    // Throws away unsaved changes and goes back to ViewMode.
    private void CancelEdit()
    {
        if (_originalTask == null)
            return;

        ErrorMessage = "";
        SuccessMessage = "";

        // Put the saved values back and lock the card again.
        LoadTask(_originalTask);
    }

    // Issue #35 - Maryam
    // Sends the edited values to the task service (UpdateTaskAsync, #39),
    // which validates them and saves them.
    private async Task SaveAsync()
    {
        // Ignore a second click while a save is already running.
        if (_isSaving || _originalTask == null)
            return;

        _isSaving = true;
        ErrorMessage = "";
        SuccessMessage = "";

        try
        {
            // The DatePicker only returns a date (midnight), so keep the
            // task's original time of day when the date is changed.
            DateTime dueDate = DueDate.HasValue
                ? DueDate.Value.Date + _originalTask.DueDate.TimeOfDay
                : default;

            var edited = new TaskItem
            {
                TaskID = TaskId,
                AgendaID = _agendaId,
                Title = Title,
                Description = Description,
                DueDate = dueDate
            };

            await _taskService.UpdateTaskAsync(edited);

            // The service trims the title, so show exactly what was stored
            // and return the card to ViewMode.
            LoadTask(edited);
            SuccessMessage = $"Task \"{edited.Title}\" was updated.";
        }
        catch (ArgumentException ex)
        {
            // Validation failed in TaskService (empty title, no due date, ...):
            // stay in edit mode so the user can fix it.
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            // Database or other unexpected problem.
            ErrorMessage = $"Could not save the task: {ex.Message}";
        }
        finally
        {
            _isSaving = false;
        }
    }
}