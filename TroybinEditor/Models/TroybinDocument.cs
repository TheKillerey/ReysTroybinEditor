namespace TroybinEditor.Models;

/// <summary>
/// Represents a loaded Troybin document.
/// </summary>
public class TroybinDocument
{
    public string FilePath   { get; set; } = string.Empty;
    public string FileName   { get; set; } = string.Empty;

    // ── Parsed data ──────────────────────────────────────────────────────

    /// <summary>Version byte (always 0x02 in known files).</summary>
    public byte Version { get; set; } = 2;

    /// <summary>All particles parsed from the file.</summary>
    public List<ParticleData> Particles { get; set; } = new();

    /// <summary>All strings from the string pool in their original order.</summary>
    public List<string> AllStrings { get; set; } = new();

    /// <summary>All decoded entries from the binary data (hash + value).</summary>
    public List<TroybinEditor.Services.IniEntry> AllEntries { get; set; } = new();

    // ── Raw binary preservation ───────────────────────────────────────────

    /// <summary>The original raw bytes read from disk (used for save-patching).</summary>
    public byte[] OriginalBytes { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// Absolute file offset where the string pool starts (= fileSize − bytes[1]).
    /// </summary>
    public int StringPoolOffset { get; set; }

    /// <summary>
    /// Absolute file offset where the uint16[] offset table starts.
    /// The table immediately precedes the string pool and begins with 0x0000.
    /// </summary>
    public int OffsetTableOffset { get; set; }

    // ── UI state ─────────────────────────────────────────────────────────

    public bool IsModified    { get; set; }
    public DateTime LastModified { get; set; } = DateTime.Now;
    public Dictionary<string, object?> Metadata { get; set; } = new();

    public TroybinDocument Clone() => new()
    {
        FilePath         = FilePath,
        FileName         = FileName,
        Version          = Version,
        Particles        = Particles.Select(p => p.Clone()).ToList(),
        AllStrings       = new List<string>(AllStrings),
        OriginalBytes    = (byte[])OriginalBytes.Clone(),
        StringPoolOffset = StringPoolOffset,
        OffsetTableOffset = OffsetTableOffset,
        IsModified       = IsModified,
        LastModified     = LastModified,
        Metadata         = new Dictionary<string, object?>(Metadata)
    };
}
