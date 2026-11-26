using System.Windows;
using System.Windows.Controls;
using TimeMirror.App.ViewModels;

namespace TimeMirror.App.Views.Dashboard;

public partial class DashboardViewUC : UserControl
{
    public DashboardViewUC()
    {
        InitializeComponent();
    }

    private async void UserControl_Loaded(
        object sender, RoutedEventArgs e)
    {
        if (DataContext is DashboardViewModel vm)
        {
            await vm.LoadAsync();
        }
    }
}