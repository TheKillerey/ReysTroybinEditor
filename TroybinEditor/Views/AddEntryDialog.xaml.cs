namespace TroybinEditor.Views;

using System.Windows;

public partial class AddEntryDialog : Window
{
    public string EntryKey   { get; private set; } = string.Empty;
    public string EntryValue { get; private set; } = string.Empty;

    public AddEntryDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => KeyBox.Focus();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        var key   = KeyBox.Text.Trim();
        var value = ValueBox.Text.Trim();
        if (string.IsNullOrEmpty(key))
        {
            KeyBox.Focus();
            return;
        }
        EntryKey    = key;
        EntryValue  = value;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}

