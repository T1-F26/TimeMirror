
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

public class AgendaViewModel : ViewModelBase
{
    private readonly Agenda? _agenda;
    private readonly UserSession _userSession;
    private readonly IAgendaService _agendaService;

    private string _selectedView = "Week";
    private string _searchText = "";

    public AgendaViewModel(
        IAgendaService agendaService,
        UserSession userSession)
    {
        _agendaService = agendaService;
        _userSession = userSession;

        // Retrieve the currently logged-in user
        int userId = _userSession.CurrentUserId;

        // Load the user's agenda
        _agenda = _agendaService.GetAgenda(userId);

        // Initialize commands
        ChangeViewCommand = new RelayCommand(
            () => SelectedView = "Day");
    }

    public string SelectedView
    {
        get => _selectedView;
        set
        {
            if (_selectedView == value)
                return;

            _selectedView = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(VisibleTasks));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;
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

            return _agenda.Tasks
                .Where(task =>
                    SelectedView switch
                    {
                        "Day" => task.DueDate.Date == today,

                        "Week" => task.DueDate.Date >= today &&
                                  task.DueDate.Date < today.AddDays(7),

                        "Month" => task.DueDate.Month == today.Month &&
                                   task.DueDate.Year == today.Year,

                        "Year" => task.DueDate.Year == today.Year,

                        _ => true
                    })
                .Where(task =>
                    string.IsNullOrWhiteSpace(SearchText) ||
                    (task.Title?.Contains(
                        SearchText,
                        StringComparison.OrdinalIgnoreCase) ?? false))
                .OrderBy(task => task.DueDate)
                .ToList();
        }
    }

    public ICommand ChangeViewCommand { get; }
}
