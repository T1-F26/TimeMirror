using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TimeMirror.App.Commands;
using TimeMirror.App.Services;
using TimeMirror.Core.Interfaces.Services;
using TimeMirror.Core.Models;

namespace TimeMirror.App.ViewModels;

// View Permissions (#105) - Vincent
// Loads the current user's permissions for PermissionViewUC (#106).
public class PermissionsViewModel : ViewModelBase
{
    private readonly IPermissionService _permissionService;
    private readonly UserSession _userSession;

    private bool _isBusy;
    private string _errorMessage = "";

    public ObservableCollection<UserPermission> Permissions { get; } = new();

    public ICommand LoadPermissions { get; }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set
        {
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public PermissionsViewModel(IPermissionService permissionService, UserSession userSession)
    {
        _permissionService = permissionService;
        _userSession = userSession;

        LoadPermissions = new RelayCommand(() => _ = LoadAsync());
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = "";

        try
        {
            var permissions = await _permissionService.GetPermissionsAsync(_userSession.CurrentUserId);

            Permissions.Clear();
            foreach (var permission in permissions)
                Permissions.Add(permission);
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load permissions. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}