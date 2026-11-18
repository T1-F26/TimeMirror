using System;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// View Task - Maryam
// Backs the reusable TaskCardUC (#53) in 'ViewMode': the card's fields are
// read-only and are filled with the task's persisted data.
// The task itself is handed in through LoadTask(...) once it has been
// retrieved (retrieval via the task service is a separate task).
public class TaskViewModel : ViewModelBase
{
    private int _taskId;
    private string _title = "";
    private string _description = "";
    private DateTime? _dueDate;
    private bool _isViewMode = true;

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

