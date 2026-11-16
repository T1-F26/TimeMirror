using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

public class AgendaViewModel : ViewModelBase
{
    private readonly Agenda? _agenda;
    private string _selectedView = "Week";

    public string SelectedView
    {
        get => _selectedView;
        set
        {
            _selectedView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(VisibleTasks));
        }
    }

    public IEnumerable<TaskItem> VisibleTasks
    {
        get
        {
            if (_agenda == null)
                return Enumerable.Empty<TaskItem>();

            var today = DateTime.Today;

            return _agenda.Tasks.Where(task =>
                SelectedView switch
                {
                    "Day" => task.DueDate.Date == today,
                    "Week" => task.DueDate.Date >= today &&
                              task.DueDate.Date < today.AddDays(7),
                    "Month" => task.DueDate.Month == today.Month &&
                               task.DueDate.Year == today.Year,
                    "Year" => task.DueDate.Year == today.Year,
                    _ => true
                });
        }
    }

    public ICommand ChangeViewCommand { get; }

    public AgendaViewModel(IAgendaService service)
    {
        _agenda = service.GetAgenda(1);

        ChangeViewCommand = new RelayCommand(
            () => SelectedView = "Day");
    }
}