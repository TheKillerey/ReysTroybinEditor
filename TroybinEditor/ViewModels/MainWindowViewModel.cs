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
    [ObservableProperty] private string statusMessage = "Ready – open a .troybin file to begin";
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string windowTitle = "Troybin Editor";

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
        _fileService   = new TroybinFileService();
        _dialogService = new FileDialogService();
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

            WindowTitle   = $"Troybin Editor – {CurrentDocument.FileName}";
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
            CurrentDocument.FilePath = path;
            CurrentDocument.FileName = Path.GetFileName(path);
            await _fileService.SaveFileAsync(CurrentDocument, path);
            WindowTitle   = $"Troybin Editor – {CurrentDocument.FileName}";
            StatusMessage = $"Saved as: {CurrentDocument.FileName}";
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
        var vm = SelectedParticle;
        var model = CurrentDocument.Particles.FirstOrDefault(p => p.Name == vm.Name);
        if (model == null) return;

        CurrentDocument.Particles.Remove(model);
        // rebuild allStrings list too
        CurrentDocument.AllStrings = CurrentDocument.Particles.SelectMany(p => p.Strings).ToList();
        CurrentDocument.IsModified = true;
        RefreshParticles();
        SelectedParticle = null;
        StatusMessage = $"Deleted particle '{vm.Name}'";
        OnPropertyChanged(nameof(FileInfoText));
    }

    // ── Internal helpers ─────────────────────────────────────────────────

    private void RefreshParticles()
    {
        Particles.Clear();
        if (CurrentDocument == null) return;
        foreach (var p in CurrentDocument.Particles)
            Particles.Add(new ParticleViewModel(p));
        if (Particles.Count > 0) SelectedParticle = Particles[0];
    }

    partial void OnSelectedParticleChanged(ParticleViewModel? value)
    {
        OnPropertyChanged(nameof(SelectedParticle));
    }
}
