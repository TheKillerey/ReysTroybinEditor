namespace TroybinEditor.Services;

using TroybinEditor.Models;

/// <summary>
/// Interface for file dialog operations.
/// </summary>
public interface IFileDialogService
{
    string? OpenFileDialog(string filter = "Troybin Files (*.troybin)|*.troybin|All Files (*.*)|*.*");
    string? SaveFileDialog(string filter = "Troybin Files (*.troybin)|*.troybin|All Files (*.*)|*.*", string defaultExt = ".troybin");
}

