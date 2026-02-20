namespace TroybinEditor.Views;

using System.Windows;

public partial class AddEmitterDialog : Window
{
    public string EmitterName { get; private set; } = string.Empty;

    public AddEmitterDialog()
    {
        InitializeComponent();
        Loaded += (_, _) => { NameBox.Focus(); NameBox.SelectAll(); };
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        var name = NameBox.Text.Trim();
        if (string.IsNullOrEmpty(name)) { NameBox.Focus(); return; }
        EmitterName  = name;
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}

