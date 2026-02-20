namespace TroybinEditor.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TroybinEditor.Models;

/// <summary>
/// ViewModel for one particle emitter group.
/// Each StringEntry represents one "key=value" field.
/// </summary>
public partial class ParticleViewModel : ObservableObject
{
    private readonly ParticleData _model;

    [ObservableProperty] private string name = string.Empty;

    public ObservableCollection<StringEntry> Entries { get; } = new();

    public ParticleViewModel(ParticleData model)
    {
        _model = model;
        Name   = model.Name;
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
            var raw = _model.Strings[i];
            // Split key=value
            int eq  = raw.IndexOf('=');
            string key = eq > 0 ? raw.Substring(0, eq) : $"S{i}";
            string val = eq > 0 ? raw.Substring(eq + 1) : raw;

            var entry = new StringEntry(i, key, val);
            int capturedIdx = i;
            entry.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(StringEntry.Value))
                    _model.Strings[capturedIdx] = $"{entry.Key}={entry.Value}";
            };
            Entries.Add(entry);
        }
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
    public int    Index  { get; }
    public string Key    { get; }
    public bool   IsName => Index == 0;
    public string Label  => Key;

    [ObservableProperty] private string value = string.Empty;

    public StringEntry(int index, string key, string value)
    {
        Index      = index;
        Key        = key;
        this.value = value;
    }
}
