using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using TroybinEditor.ViewModels;
using TroybinEditor.Views;

namespace TroybinEditor;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel();

        // Ctrl+Z → Reset selected particle to original values
        InputBindings.Add(new KeyBinding(
            new RelayCommand(() => ((MainWindowViewModel)DataContext).ResetSelectedParticleCommand.Execute(null)),
            new KeyGesture(Key.Z, ModifierKeys.Control)));
    }

    private void MenuItem_Exit(object sender, RoutedEventArgs e) => Close();

    private void OpenConverter_Click(object sender, RoutedEventArgs e)
    {
        var win = new ConvertWindow { Owner = this };
        win.Show(); // non-modal so user can reference the editor
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        if (vm?.CurrentDocument == null || !vm.CurrentDocument.IsModified) return;

        var result = MessageBox.Show(
            "You have unsaved changes.\n\nSave before closing?",
            "Unsaved Changes",
            MessageBoxButton.YesNoCancel,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            // Fire save command synchronously (it's async, so we use a dispatcher trick)
            vm.SaveFileCommand.Execute(null);
            // Note: we don't cancel – save runs async and window closes. Document is written.
        }
        else if (result == MessageBoxResult.Cancel)
        {
            e.Cancel = true; // stay open
        }
        // No → just close
    }

    /// <summary>Minimal ICommand wrapper for key bindings.</summary>
    private sealed class RelayCommand : ICommand
    {
        private readonly Action _action;
        public RelayCommand(Action action) => _action = action;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _action();
    }
}
