using System.Windows;
using System.Windows.Controls;
using TimeMirror.App.ViewModels;

namespace TimeMirror.App.Views.Permissions
{
    /// <summary>
    /// View Permissions page (#106).
    /// On load, asks PermissionsViewModel (#105) to load the user's permissions.
    /// </summary>
    public partial class PermissionViewUC : UserControl
    {
        public PermissionViewUC()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is PermissionsViewModel vm)
                await vm.LoadAsync();
        }
    }
}