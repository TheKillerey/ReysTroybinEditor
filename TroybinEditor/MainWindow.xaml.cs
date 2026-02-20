using System.Windows;
using System.Windows.Input;
using TroybinEditor.ViewModels;

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

