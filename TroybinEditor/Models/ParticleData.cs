namespace TroybinEditor.Models;

/// <summary>
/// Represents one particle emitter group in a Troybin file.
/// Strings contains "fieldName=value" pairs resolved from the INIBIN hash dictionary.
/// </summary>
public class ParticleData
{
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// All entries for this emitter in "key=value" format.
    /// Index 0 is special: always the group name (the Name property itself).
    /// </summary>
    public List<string> Strings { get; set; } = new();

    // ── Typed convenience accessors (scan Strings for known keys) ────────────

    public string? Texture =>
        GetValue("p-texture") ?? GetValue("p-meshtex");

    public string? Mesh =>
        GetValue("p-mesh");

    public string? BlendMode =>
        GetValue("rendermode") ?? GetValue("pass");

    private string? GetValue(string key)
    {
        foreach (var s in Strings)
        {
            int eq = s.IndexOf('=');
            if (eq > 0 && s.Substring(0, eq).Trim().Equals(key, StringComparison.OrdinalIgnoreCase))
                return s.Substring(eq + 1).Trim();
        }
        return null;
    }

    // ── Property dictionary view ─────────────────────────────────────────────

    public Dictionary<string, string> Properties
    {
        get
        {
            var d = new Dictionary<string, string>();
            for (int i = 0; i < Strings.Count; i++)
                d[$"S{i}"] = Strings[i];
            return d;
        }
    }

    public void SetProperty(string key, string value)
    {
        if (key.Length > 1 && key[0] == 'S' && int.TryParse(key.AsSpan(1), out int idx)
            && idx >= 0 && idx < Strings.Count)
            Strings[idx] = value;
    }

    public ParticleData Clone() => new()
    {
        Name    = Name,
        Strings = new List<string>(Strings)
    };
}
