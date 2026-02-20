namespace TroybinEditor.Services;

using System.IO;
using System.Text.Json;

/// <summary>
/// Service for application settings and preferences.
/// </summary>
public class SettingsService
{
    private readonly string _settingsPath;
    private Dictionary<string, object?> _settings;
    
    public SettingsService()
    {
        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TroybinEditor",
            "settings.json");
        
        _settings = new Dictionary<string, object?>();
        LoadSettings();
    }
    
    public T? Get<T>(string key, T? defaultValue = default)
    {
        if (_settings.TryGetValue(key, out var value))
        {
            return (T?)Convert.ChangeType(value, typeof(T));
        }
        return defaultValue;
    }
    
    public void Set<T>(string key, T? value)
    {
        _settings[key] = value;
        SaveSettings();
    }
    
    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                var loaded = JsonSerializer.Deserialize<Dictionary<string, object?>> (json);
                if (loaded != null)
                    _settings = loaded;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }
    }
    
    private void SaveSettings()
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);
            
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
}

