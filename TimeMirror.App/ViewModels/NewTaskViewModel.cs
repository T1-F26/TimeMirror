using System;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// Issue #49 - Vincent
// Holds what the user types in the New Task form,
// sends it to TaskService to validate + save,
// and exposes success/error messages for the View.
public class NewTaskViewModel : ViewModelBase
{
    // Simulated logged-in user's agenda until authentication exists.
    private const int CurrentAgendaId = 1;

    private readonly ITaskService _taskService;

    private string _title = "";
    private string _description = "";
    private DateTime? _dueDate = DateTime.Today;
    private string _errorMessage = "";
    private string _successMessage = "";

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

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set { _successMessage = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    // TaskService comes in through dependency injection.
    public NewTaskViewModel(ITaskService taskService)
    {
        _taskService = taskService;

        SaveCommand = new RelayCommand(Save);
        ClearCommand = new RelayCommand(Clear);
    }

    private void Save()
    {
        ErrorMessage = "";
        SuccessMessage = "";

        try
        {
            var task = new TaskItem
            {
                AgendaID = CurrentAgendaId,
                Title = Title,
                Description = Description,
                DueDate = DueDate ?? default
            };

            var saved = _taskService.CreateTask(task);

            ResetFields();
            SuccessMessage = $"Task \"{saved.Title}\" was added.";
        }
        catch (ArgumentException ex)
        {
            // Validation failed in TaskService: show the reason to the user.
            ErrorMessage = ex.Message;
        }
    }

    private void Clear()
    {
        ResetFields();
        ErrorMessage = "";
        SuccessMessage = "";
    }

    private void ResetFields()
    {
        Title = "";
        Description = "";
        DueDate = DateTime.Today;
    }
}