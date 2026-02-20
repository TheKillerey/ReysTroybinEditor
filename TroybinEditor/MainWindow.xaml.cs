using System.Windows;
using TroybinEditor.ViewModels;

namespace TroybinEditor;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();
    }
    
    private void MenuItem_Exit(object sender, RoutedEventArgs e)
    {
        Close();
    }
}