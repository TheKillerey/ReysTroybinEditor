namespace TroybinEditor.ViewModels;

using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TroybinEditor.Models;
using TroybinEditor.Services;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ITroybinFileService _fileService;
    private readonly IFileDialogService  _dialogService;
    private readonly IMessageService     _messageService;

    [ObservableProperty] private TroybinDocument? currentDocument;
    [ObservableProperty] private ParticleViewModel? selectedParticle;
    [ObservableProperty] private StringEntry?       selectedEntry;
    [ObservableProperty] private string statusMessage = "Ready – open a .troybin file to begin";
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string windowTitle = "Rey's Troybin Editor";

    public ObservableCollection<ParticleViewModel> Particles { get; } = new();

    // ── File info for the info panel ─────────────────────────────────────
    public string FileInfoText
    {
        get
        {
            if (CurrentDocument == null) return "No file loaded";
            return $"File: {CurrentDocument.FileName}\n" +
                   $"Size: {CurrentDocument.OriginalBytes.Length} bytes\n" +
                   $"Version: 0x{CurrentDocument.Version:X2}\n" +
                   $"String pool: offset 0x{CurrentDocument.StringPoolOffset:X}  ({CurrentDocument.AllStrings.Count} strings)\n" +
                   $"Particles: {CurrentDocument.Particles.Count}";
        }
    }

    public MainWindowViewModel()
    {
        _fileService    = new TroybinFileService();
        _dialogService  = new FileDialogService();
        _messageService = new MessageService();
    }

    // ── Commands ──────────────────────────────────────────────────────────

    [RelayCommand]
    public async Task OpenFile()
    {
        try
        {
            var filePath = _dialogService.OpenFileDialog();
            if (string.IsNullOrEmpty(filePath)) return;

            IsLoading     = true;
            StatusMessage = $"Loading {Path.GetFileName(filePath)}…";

            CurrentDocument = await _fileService.LoadFileAsync(filePath);
            RefreshParticles();

            WindowTitle   = $"Rey's Troybin Editor – {CurrentDocument.FileName}";
            StatusMessage = $"Loaded: {CurrentDocument.FileName}  ({CurrentDocument.Particles.Count} particles, {CurrentDocument.AllStrings.Count} strings)";
            OnPropertyChanged(nameof(FileInfoText));
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            _messageService.ShowError("Load Error", ex.Message);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task SaveFile()
    {
        if (CurrentDocument == null)
        { _messageService.ShowWarning("Nothing to save", "Open a file first."); return; }
        try
        {
            IsLoading     = true;
            StatusMessage = "Saving…";
            SyncDocumentFromViewModels();
            await _fileService.SaveFileAsync(CurrentDocument, CurrentDocument.FilePath);
            StatusMessage = $"Saved: {CurrentDocument.FileName}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
            _messageService.ShowError("Save Error", ex.Message);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public async Task SaveAs()
    {
        if (CurrentDocument == null)
        { _messageService.ShowWarning("Nothing to save", "Open a file first."); return; }
        try
        {
            var path = _dialogService.SaveFileDialog();
            if (string.IsNullOrEmpty(path)) return;

            IsLoading     = true;
            StatusMessage = "Saving…";
            SyncDocumentFromViewModels();
            CurrentDocument.FilePath = path;
            CurrentDocument.FileName = Path.GetFileName(path);
            await _fileService.SaveFileAsync(CurrentDocument, path);
            WindowTitle   = $"Rey's Troybin Editor – {CurrentDocument.FileName}";
            StatusMessage = $"Saved as: {CurrentDocument.FileName}";
            OnPropertyChanged(nameof(FileInfoText));
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save error: {ex.Message}";
            _messageService.ShowError("Save Error", ex.Message);
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    public void DeleteSelectedParticle()
    {
        if (SelectedParticle == null || CurrentDocument == null) return;
        var vm   = SelectedParticle;
        var model = CurrentDocument.Particles.FirstOrDefault(p => p.Name == vm.Name);
        if (model == null) return;

        CurrentDocument.Particles.Remove(model);
        CurrentDocument.AllStrings = CurrentDocument.Particles.SelectMany(p => p.Strings).ToList();
        CurrentDocument.IsModified = true;
        RefreshParticles();
        SelectedParticle = null;
        StatusMessage    = $"Deleted emitter '{vm.Name}'";
        OnPropertyChanged(nameof(FileInfoText));
    }

    /// <summary>Reset selected particle's values back to what was loaded from disk.</summary>
    [RelayCommand]
    public void ResetSelectedParticle()
    {
        if (SelectedParticle == null) return;
        SelectedParticle.ResetToOriginal();
        // Also refresh the document's AllStrings
        if (CurrentDocument != null)
            CurrentDocument.AllStrings = CurrentDocument.Particles.SelectMany(p => p.Strings).ToList();
        StatusMessage = $"Reset '{SelectedParticle.Name}' to original values";
    }

    /// <summary>Delete the selected string entry from the selected particle.</summary>
    [RelayCommand]
    public void DeleteSelectedEntry()
    {
        if (SelectedParticle == null || SelectedEntry == null) return;
        var entry = SelectedEntry;
        SelectedEntry = null;
        SelectedParticle.DeleteEntry(entry);
        if (CurrentDocument != null)
            CurrentDocument.IsModified = true;
        StatusMessage = $"Deleted entry '{entry.Key}'";
        OnPropertyChanged(nameof(FileInfoText));
    }

    /// <summary>Opens the Add Entry dialog for the selected particle.</summary>
    [RelayCommand]
    public void AddEntry()
    {
        if (SelectedParticle == null) return;
        var dlg = new Views.AddEntryDialog { Owner = GetMainWindow() };
        if (dlg.ShowDialog() != true) return;

        SelectedParticle.AddEntry(dlg.EntryKey, dlg.EntryValue);
        if (CurrentDocument != null)
            CurrentDocument.IsModified = true;
        StatusMessage = $"Added entry '{dlg.EntryKey}' to '{SelectedParticle.Name}'";
    }

    /// <summary>Opens the Add Emitter dialog to create a new emitter in the document.</summary>
    [RelayCommand]
    public void AddEmitter()
    {
        if (CurrentDocument == null)
        { _messageService.ShowWarning("No file", "Open a file first."); return; }

        var dlg = new Views.AddEmitterDialog { Owner = GetMainWindow() };
        if (dlg.ShowDialog() != true) return;

        var newParticle = new ParticleData { Name = dlg.EmitterName };
        CurrentDocument.Particles.Add(newParticle);
        CurrentDocument.IsModified = true;
        RefreshParticles();
        SelectedParticle = Particles.LastOrDefault();
        StatusMessage    = $"Added emitter '{dlg.EmitterName}'";
        OnPropertyChanged(nameof(FileInfoText));
    }

    /// <summary>Export the current document to a human-readable .txt file.</summary>
    [RelayCommand]
    public async Task ExportToText()
    {
        if (CurrentDocument == null)
        { _messageService.ShowWarning("No file", "Open a file first."); return; }
        try
        {
            var path = _dialogService.SaveFileDialog("Text files (*.txt)|*.txt", ".txt");
            if (string.IsNullOrEmpty(path)) return;

            SyncDocumentFromViewModels();
            await TroybinTextConverter.ExportToFileAsync(CurrentDocument, path);
            StatusMessage = $"Exported to: {Path.GetFileName(path)}";
        }
        catch (Exception ex)
        {
            _messageService.ShowError("Export Error", ex.Message);
        }
    }

    /// <summary>Import from a previously exported text file and update the document.</summary>
    [RelayCommand]
    public async Task ImportFromText()
    {
        if (CurrentDocument == null)
        { _messageService.ShowWarning("No file", "Open a file first."); return; }
        try
        {
            var path = _dialogService.OpenFileDialog("Text files (*.txt)|*.txt");
            if (string.IsNullOrEmpty(path)) return;

            IsLoading     = true;
            StatusMessage = "Importing…";

            var updated     = await TroybinTextConverter.ImportFromFileAsync(path, CurrentDocument);
            CurrentDocument = updated;
            RefreshParticles();
            StatusMessage   = $"Imported from: {Path.GetFileName(path)}";
            OnPropertyChanged(nameof(FileInfoText));
        }
        catch (Exception ex)
        {
            _messageService.ShowError("Import Error", ex.Message);
        }
        finally { IsLoading = false; }
    }

    // ── Internal helpers ─────────────────────────────────────────────────

    /// <summary>
    /// Ensures the document's Particles list reflects all ViewModel edits before saving.
    /// The ParticleViewModel already writes back to the model on each change,
    /// but this is a safety flush.
    /// </summary>
    private void SyncDocumentFromViewModels()
    {
        if (CurrentDocument == null) return;
        CurrentDocument.AllStrings = CurrentDocument.Particles.SelectMany(p => p.Strings).ToList();
    }

    private void RefreshParticles()
    {
        Particles.Clear();
        if (CurrentDocument == null) return;
        foreach (var p in CurrentDocument.Particles)
            Particles.Add(new ParticleViewModel(p));
        if (Particles.Count > 0) SelectedParticle = Particles[0];
    }

    private System.Windows.Window? GetMainWindow()
        => System.Windows.Application.Current.MainWindow;

    partial void OnSelectedParticleChanged(ParticleViewModel? value)
    {
        SelectedEntry = null;
        OnPropertyChanged(nameof(SelectedParticle));
    }
}

