namespace TroybinEditor.Services;

using System.Windows;
using Microsoft.Win32;

/// <summary>
/// Implements file dialog operations using WPF dialogs.
/// </summary>
public class FileDialogService : IFileDialogService
{
    public string? OpenFileDialog(string filter = "Troybin Files (*.troybin)|*.troybin|All Files (*.*)|*.*")
    {
        var dialog = new OpenFileDialog { Filter = filter };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
    
    public string? SaveFileDialog(string filter = "Troybin Files (*.troybin)|*.troybin|All Files (*.*)|*.*", string defaultExt = ".troybin")
    {
        var dialog = new SaveFileDialog { Filter = filter, DefaultExt = defaultExt };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}

