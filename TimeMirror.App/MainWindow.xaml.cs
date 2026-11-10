using System.Windows;
using TimeMirror.App.ViewModels;

namespace TimeMirror.App;

public partial class MainWindow : Window
{
    public MainWindow(ShellViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}