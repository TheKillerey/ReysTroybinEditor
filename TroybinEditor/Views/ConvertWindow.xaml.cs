namespace TroybinEditor.Views;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TroybinEditor.Services;

public partial class ConvertWindow : Window
{
    private readonly ObservableCollection<string> _files = new();
    private string? _outputFolder;

    public ConvertWindow()
    {
        InitializeComponent();
        FileListBox.ItemsSource = _files;
    }

    private void Mode_Changed(object sender, RoutedEventArgs e)
    {
        if (FileListLabel == null) return;
        bool toText = RadioToText.IsChecked == true;
        FileListLabel.Text  = toText ? "INPUT FILES (.troybin)" : "INPUT FILES (.txt)";
        AddFilesBtn.Content = toText ? "➕ Add Files" : "➕ Add Text Files";
        _files.Clear();
        LogBox.Text = string.Empty;
        UpdateStatus();
    }

    private void AddFiles_Click(object sender, RoutedEventArgs e)
    {
        bool toText   = RadioToText.IsChecked == true;
        string filter = toText
            ? "Troybin Files (*.troybin)|*.troybin|All Files (*.*)|*.*"
            : "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
        var dlg = new Microsoft.Win32.OpenFileDialog { Filter = filter, Multiselect = true };
        if (dlg.ShowDialog() != true) return;
        foreach (var f in dlg.FileNames)
            if (!_files.Contains(f)) _files.Add(f);
        UpdateStatus();
    }

    private void AddFolder_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select folder to scan" };
        if (dlg.ShowDialog() != true) return;
        string ext = RadioToText.IsChecked == true ? "*.troybin" : "*.txt";
        foreach (var f in Directory.GetFiles(dlg.FolderName, ext, SearchOption.AllDirectories))
            if (!_files.Contains(f)) _files.Add(f);
        UpdateStatus();
    }

    private void ClearFiles_Click(object sender, RoutedEventArgs e)
    {
        _files.Clear();
        LogBox.Text = string.Empty;
        UpdateStatus();
    }

    private void BrowseOutput_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFolderDialog { Title = "Select output folder" };
        if (dlg.ShowDialog() != true) return;
        _outputFolder                = dlg.FolderName;
        OutputFolderLabel.Text       = _outputFolder;
        OutputFolderLabel.Foreground = (Brush)FindResource("TextBrush");
    }

    private void FileList_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy : DragDropEffects.None;
        e.Handled = true;
    }

    private void FileList_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        if (e.Data.GetData(DataFormats.FileDrop) is not string[] dropped) return;
        string ext = RadioToText.IsChecked == true ? ".troybin" : ".txt";
        foreach (var path in dropped)
        {
            if (Directory.Exists(path))
                foreach (var f in Directory.GetFiles(path, "*" + ext, SearchOption.AllDirectories))
                { if (!_files.Contains(f)) _files.Add(f); }
            else if (File.Exists(path) && path.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                if (!_files.Contains(path)) _files.Add(path);
        }
        UpdateStatus();
    }

    private async void Convert_Click(object sender, RoutedEventArgs e)
    {
        if (_files.Count == 0) { AppendLog("⚠  No files added."); return; }
        ConvertBtn.IsEnabled = false;
        LogBox.Text          = string.Empty;
        bool toText          = RadioToText.IsChecked == true;
        int  ok = 0, fail = 0;

        foreach (var inputFile in _files.ToList())
        {
            try
            {
                string outDir  = _outputFolder ?? Path.GetDirectoryName(inputFile)!;
                string outName = toText
                    ? Path.ChangeExtension(Path.GetFileName(inputFile), ".txt")
                    : Path.ChangeExtension(Path.GetFileName(inputFile), ".troybin");
                string outPath = Path.Combine(outDir, outName);

                if (toText)
                {
                    var svc = new TroybinFileService();
                    var doc = await svc.LoadFileAsync(inputFile);
                    await TroybinTextConverter.ExportToFileAsync(doc, outPath);
                    AppendLog($"✅  {Path.GetFileName(inputFile)}  →  {outPath}");
                }
                else
                {
                    var baseName   = Path.GetFileNameWithoutExtension(inputFile);
                    var srcTroybin = Path.Combine(Path.GetDirectoryName(inputFile)!, baseName + ".troybin");
                    if (!File.Exists(srcTroybin))
                    {
                        AppendLog($"⚠  {Path.GetFileName(inputFile)}  — no matching .troybin found next to it");
                        fail++; continue;
                    }
                    var svc     = new TroybinFileService();
                    var orig    = await svc.LoadFileAsync(srcTroybin);
                    var updated = await TroybinTextConverter.ImportFromFileAsync(inputFile, orig);
                    await svc.SaveFileAsync(updated, outPath);
                    AppendLog($"✅  {Path.GetFileName(inputFile)}  →  {outPath}");
                }
                ok++;
            }
            catch (Exception ex)
            {
                AppendLog($"❌  {Path.GetFileName(inputFile)}  —  {ex.Message}");
                fail++;
            }
        }
        AppendLog($"\nDone: {ok} succeeded, {fail} failed.");
        StatusLabel.Text     = $"Done: {ok} succeeded, {fail} failed.";
        ConvertBtn.IsEnabled = true;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void UpdateStatus()
        => StatusLabel.Text = _files.Count == 0 ? "Add files and click Convert" : $"{_files.Count} file(s) ready";

    private void AppendLog(string msg)
    {
        LogBox.Text += msg + "\n";
        LogScroller.ScrollToBottom();
    }
}
