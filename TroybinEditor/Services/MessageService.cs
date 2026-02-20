namespace TroybinEditor.Services;

using System.Windows;

/// <summary>
/// Service for showing user dialogs and notifications.
/// </summary>
public interface IMessageService
{
    void ShowInfo(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
    bool ShowQuestion(string title, string message);
}

/// <summary>
/// Implements message dialogs using WPF MessageBox.
/// </summary>
public class MessageService : IMessageService
{
    public void ShowInfo(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    public void ShowWarning(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
    }
    
    public void ShowError(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
    
    public bool ShowQuestion(string title, string message)
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }
}

