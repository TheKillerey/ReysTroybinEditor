namespace TroybinEditor.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TroybinEditor.Models;

/// <summary>
/// ViewModel for one particle emitter group.
/// Each StringEntry represents one "key=value" field.
/// </summary>
public partial class ParticleViewModel : ObservableObject
{
    private readonly ParticleData _model;

    /// <summary>Snapshot of the original strings for Undo/Reset.</summary>
    private readonly List<string> _originalStrings;
    private readonly string _originalName;

    /// <summary>Called whenever any entry value is changed inline (for IsModified tracking).</summary>
    public Action? OnModified { get; set; }

    [ObservableProperty] private string name = string.Empty;

    public ObservableCollection<StringEntry> Entries { get; } = new();

    public ParticleViewModel(ParticleData model)
    {
        _model           = model;
        _originalStrings = new List<string>(model.Strings);
        _originalName    = model.Name;
        Name             = model.Name;
        Rebuild();
    }

    partial void OnNameChanged(string value)
    {
        _model.Name = value;
    }

    public void Rebuild()
    {
        Entries.Clear();
        for (int i = 0; i < _model.Strings.Count; i++)
        {
            var raw   = _model.Strings[i];
            int eq    = raw.IndexOf('=');
            string key = eq > 0 ? raw.Substring(0, eq) : $"S{i}";
            string val = eq > 0 ? raw.Substring(eq + 1) : raw;

            var entry = new StringEntry(i, key, val, this);
            Entries.Add(entry);
        }
    }

    /// <summary>Called by StringEntry when its Value changes – syncs back to model.</summary>
    internal void OnEntryValueChanged(StringEntry entry)
    {
        if (entry.Index >= 0 && entry.Index < _model.Strings.Count)
            _model.Strings[entry.Index] = $"{entry.Key}={entry.Value}";
        OnModified?.Invoke();
    }

    /// <summary>Reset all entries to the values they had when the file was loaded.</summary>
    public void ResetToOriginal()
    {
        _model.Name    = _originalName;
        _model.Strings = new List<string>(_originalStrings);
        Name           = _originalName;
        Rebuild();
        OnPropertyChanged(nameof(StringCount));
        OnPropertyChanged(nameof(Texture));
        OnPropertyChanged(nameof(Mesh));
        OnPropertyChanged(nameof(BlendMode));
        OnPropertyChanged(nameof(PreviewLine));
    }

    /// <summary>Add a new key=value entry to this emitter.</summary>
    public void AddEntry(string key, string value)
    {
        var raw = $"{key}={value}";
        _model.Strings.Add(raw);
        var entry = new StringEntry(_model.Strings.Count - 1, key, value, this);
        Entries.Add(entry);
        OnPropertyChanged(nameof(StringCount));
    }

    /// <summary>Delete a specific entry from this emitter.</summary>
    public void DeleteEntry(StringEntry entry)
    {
        if (entry.Index < 0 || entry.Index >= _model.Strings.Count) return;
        _model.Strings.RemoveAt(entry.Index);
        // Rebuild to refresh all indices
        Rebuild();
        OnPropertyChanged(nameof(StringCount));
        OnPropertyChanged(nameof(Texture));
        OnPropertyChanged(nameof(Mesh));
        OnPropertyChanged(nameof(BlendMode));
        OnPropertyChanged(nameof(PreviewLine));
    }

    public string? Texture   => _model.Texture;
    public string? Mesh      => _model.Mesh;
    public string? BlendMode => _model.BlendMode;

    public string PreviewLine
    {
        get
        {
            var parts = new List<string?>();
            if (!string.IsNullOrEmpty(Mesh))      parts.Add($"mesh:{Mesh}");
            if (!string.IsNullOrEmpty(Texture))   parts.Add($"tex:{Texture}");
            if (!string.IsNullOrEmpty(BlendMode)) parts.Add(BlendMode);
            return parts.Count > 0 ? string.Join("  ·  ", parts) : string.Empty;
        }
    }

    public int StringCount => _model.Strings.Count;
}

/// <summary>A single editable key=value entry in a particle emitter group.</summary>
public partial class StringEntry : ObservableObject
{
    private readonly ParticleViewModel _owner;

    public int    Index  { get; }
    public string Key    { get; }
    public bool   IsName => Index == 0;
    public string Label  => Key;

    [ObservableProperty] private string value = string.Empty;

    public StringEntry(int index, string key, string value, ParticleViewModel owner)
    {
        _owner     = owner;
        Index      = index;
        Key        = key;
        this.value = value;
    }

    partial void OnValueChanged(string value)
    {
        _owner.OnEntryValueChanged(this);
    }
}
