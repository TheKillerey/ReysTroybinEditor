namespace TroybinEditor.Services;

using System.IO;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Service for managing recently opened files.
/// </summary>
public class RecentFilesService
{
    private readonly string _recentFilesPath;
    private readonly List<string> _recentFiles;
    private const int MaxRecentFiles = 10;
    
    public IReadOnlyList<string> RecentFiles => _recentFiles.AsReadOnly();
    
    public RecentFilesService()
    {
        _recentFilesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TroybinEditor",
            "recent.txt");
        
        _recentFiles = new List<string>();
        LoadRecentFiles();
    }
    
    public void AddFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            return;
        
        // Entferne Duplikat wenn vorhanden
        _recentFiles.Remove(filePath);
        
        // Füge am Anfang hinzu
        _recentFiles.Insert(0, filePath);
        
        // Behalte nur die letzten MaxRecentFiles
        while (_recentFiles.Count > MaxRecentFiles)
            _recentFiles.RemoveAt(_recentFiles.Count - 1);
        
        SaveRecentFiles();
    }
    
    public void RemoveFile(string filePath)
    {
        if (_recentFiles.Remove(filePath))
            SaveRecentFiles();
    }
    
    public void ClearAll()
    {
        _recentFiles.Clear();
        SaveRecentFiles();
    }
    
    private void LoadRecentFiles()
    {
        try
        {
            if (File.Exists(_recentFilesPath))
            {
                var lines = File.ReadAllLines(_recentFilesPath);
                _recentFiles.AddRange(lines.Where(f => File.Exists(f)));
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recent files: {ex.Message}");
        }
    }
    
    private void SaveRecentFiles()
    {
        try
        {
            var directory = Path.GetDirectoryName(_recentFilesPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);
            
            File.WriteAllLines(_recentFilesPath, _recentFiles);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving recent files: {ex.Message}");
        }
    }
}

