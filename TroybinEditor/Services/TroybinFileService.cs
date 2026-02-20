namespace TroybinEditor.Services;

using System.IO;
using System.Text;
using TroybinEditor.Models;

/// <summary>
/// Reads and writes the League of Legends .troybin (INIBIN v2) particle file format.
///
/// REAL format (reverse-engineered from Leischii's TroybinConverter / Main.jsx):
///
///   Byte 0          : version  (2 = new "INIBIN v2", 1 = old)
///
///   ── Version 2 layout ──────────────────────────────────────────────────────
///   uint16          : stringsLength  (byte count of the raw string data block)
///   uint16          : flags          (bitmask – which typed blocks follow)
///
///   For each bit i (0-15) set in flags, a typed block is present:
///     Bit  0 : int32   (fmt "<i", count=1, mul=1)
///     Bit  1 : float32 (fmt "<f", count=1, mul=1)
///     Bit  2 : uint8   (fmt "<B", count=1, mul=0.1)
///     Bit  3 : int16   (fmt "<h", count=1, mul=1)
///     Bit  4 : uint8   (fmt "<B", count=1, mul=1)
///     Bit  5 : bool    (packed, special reader)
///     Bit  6 : uint8[3] (fmt "<B", count=3, mul=0.1)
///     Bit  7 : float[3]  (fmt "<f", count=3, mul=1)
///     Bit  8 : uint8[2]  (fmt "<B", count=2, mul=0.1)
///     Bit  9 : float[2]  (fmt "<f", count=2, mul=1)
///     Bit 10 : uint8[4]  (fmt "<B", count=4, mul=0.1)   ← colors
///     Bit 11 : float[4]  (fmt "<f", count=4, mul=1)
///     Bit 12 : strings   (special: uint16 offsets block + raw string bytes)
///     Bit 13 : int64
///
///   Every numeric/string block has this header:
///     uint16  count
///     uint32[count]  hashes   (ihash of the field name)
///   Followed by count × values.
///
///   Bool block:
///     uint16  count
///     uint32[count]  hashes
///     ceil(count/8) bytes of packed booleans
///
///   String block:
///     uint16  count         (number of string entries)
///     uint32[count]  hashes
///     uint16[count]  offsets (into the raw string data)
///     byte[stringsLength]  raw null-terminated string data
///
///   Field names are identified by their ihash.  The dictionary provides ~1000
///   known name→hash mappings so most fields can be labelled.
/// </summary>
public class TroybinFileService : ITroybinFileService
{
    // ── Inibin ihash (same algorithm as Leischii's Main.jsx) ────────────────

    private static uint IHash(string value, uint ret = 0)
    {
        foreach (char c in value)
        {
            ret = (uint)(((char.ToLowerInvariant(c) + (65599u * ret)) & 0xFFFFFFFF));
        }
        return ret;
    }

    private static uint SectionFieldHash(string section, string fieldName)
    {
        uint sectionHash = IHash("*", IHash(section));
        return IHash(fieldName, sectionHash);
    }

    // ── Load ────────────────────────────────────────────────────────────────

    public async Task<TroybinDocument> LoadFileAsync(string filePath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var bytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
        if (bytes.Length < 4)
            throw new IOException("File too small to be a valid Troybin file.");

        var doc = new TroybinDocument
        {
            FilePath      = filePath,
            FileName      = Path.GetFileName(filePath),
            Version       = bytes[0],
            OriginalBytes = bytes
        };

        int pos = 0;
        byte version = bytes[pos++];
        doc.Version = version;

        if (version == 2)
            ParseV2(bytes, ref pos, doc);
        else if (version == 1)
            ParseV1(bytes, ref pos, doc);
        else
            throw new IOException($"Unknown Troybin version: {version}");

        doc.IsModified = false;
        return doc;
    }

    // ── Save ────────────────────────────────────────────────────────────────

    public async Task SaveFileAsync(TroybinDocument document, string filePath,
        CancellationToken cancellationToken = default)
    {
        if (File.Exists(filePath))
            File.Copy(filePath, filePath + ".bak", overwrite: true);

        var patched = BuildPatched(document);
        await File.WriteAllBytesAsync(filePath, patched, cancellationToken);

        document.IsModified   = false;
        document.LastModified = DateTime.Now;
    }

    public bool ValidateDocument(TroybinDocument document) =>
        !string.IsNullOrEmpty(document.FileName) && document.AllStrings.Count > 0;

    // ── V2 Parser ────────────────────────────────────────────────────────────

    private static void ParseV2(byte[] b, ref int pos, TroybinDocument doc)
    {
        ushort stringsLength = ReadU16(b, ref pos);
        ushort flags         = ReadU16(b, ref pos);

        if (flags == 0)
            flags = ReadU16(b, ref pos);

        doc.Metadata["StrLen"] = stringsLength;
        doc.Metadata["Flags"]  = flags;

        var entries = new List<IniEntry>();

        for (int bit = 0; bit < 14; bit++)
        {
            if ((flags & (1 << bit)) == 0) continue;
            switch (bit)
            {
                case 5:  entries.AddRange(ReadBools(b, ref pos)); break;
                case 12: entries.AddRange(ReadStrings(b, ref pos, stringsLength)); break;
                default: entries.AddRange(ReadNumbers(b, ref pos, bit)); break;
            }
        }

        doc.AllEntries = entries;

        // ── Pass 1: resolve using System-only hash map to extract group names ──
        var systemMap = IniHashDictionary.BuildHashMap(null);
        foreach (var e in entries)
        {
            if (systemMap.TryGetValue(e.Hash, out var label))
            {
                var (sec, fld) = IniHashDictionary.ParseLabel(label);
                e.ResolvedSection = sec;
                e.ResolvedName    = fld;
            }
        }

        // Extract actual emitter names from System[GroupPartN] entries
        var groupNames = entries
            .Where(e => e.ResolvedSection == "System" &&
                        e.ResolvedName != null &&
                        e.ResolvedName.StartsWith("GroupPart") &&
                        !e.ResolvedName.Contains("Type") &&
                        !e.ResolvedName.Contains("Importance") &&
                        e.Value is string s && !string.IsNullOrEmpty(s))
            .Select(e => (string)e.Value!)
            .Distinct()
            .ToList();

        doc.Metadata["GroupNames"] = string.Join(", ", groupNames);

        // ── Pass 2: rebuild map with actual group names and re-resolve ──────
        var fullMap = IniHashDictionary.BuildHashMap(groupNames);
        foreach (var e in entries)
        {
            if (fullMap.TryGetValue(e.Hash, out var label))
            {
                var (sec, fld) = IniHashDictionary.ParseLabel(label);
                e.ResolvedSection = sec;
                e.ResolvedName    = fld;
            }
        }

        doc.AllStrings = entries
            .Where(e => e.Value is string)
            .Select(e => e.Value as string ?? "")
            .ToList();

        doc.Particles = BuildParticles(entries, fullMap);

        doc.Metadata["EntryCount"] = entries.Count;
        doc.Metadata["FileSize"]   = b.Length;
    }

    // ── V1 Parser ────────────────────────────────────────────────────────────

    private static void ParseV1(byte[] b, ref int pos, TroybinDocument doc)
    {
        // Skip 3 unknown bytes
        pos += 3;
        uint entryCount = ReadU32(b, ref pos);
        uint dataCount  = ReadU32(b, ref pos);

        var offsets      = new Dictionary<uint, uint>();
        var offsetIndices = new List<uint>();

        for (int i = 0; i < (int)entryCount; i++)
        {
            uint h = ReadU32(b, ref pos);
            uint o = ReadU32(b, ref pos);
            offsets[h] = o;
            offsetIndices.Add(h);
        }

        var data = b.Skip(pos).Take((int)dataCount).ToArray();
        pos += (int)dataCount;

        var entries = new List<IniEntry>();
        foreach (var hash in offsetIndices)
        {
            int o = (int)offsets[hash];
            var sb = new StringBuilder();
            while (o < data.Length && data[o] != 0) sb.Append((char)data[o++]);
            entries.Add(new IniEntry { Hash = hash, Value = SanitizeString(sb.ToString()) });
        }

        doc.AllEntries = entries;
        doc.AllStrings = entries.Where(e => e.Value is string).Select(e => (string)e.Value!).ToList();
        var v1Map = IniHashDictionary.BuildHashMap(null);
        foreach (var e in entries)
            if (v1Map.TryGetValue(e.Hash, out var lbl))
            { var (s,f) = IniHashDictionary.ParseLabel(lbl); e.ResolvedSection=s; e.ResolvedName=f; }
        doc.Particles  = BuildParticles(entries, v1Map);
    }

    // ── Block readers ────────────────────────────────────────────────────────

    private static List<IniEntry> ReadBools(byte[] b, ref int pos)
    {
        var result  = new List<IniEntry>();
        ushort num  = ReadU16(b, ref pos);
        var keys    = new uint[num];
        for (int i = 0; i < num; i++) keys[i] = ReadU32(b, ref pos);

        int byteCount = (num / 8) + (num % 8 > 0 ? 1 : 0);
        for (int j = 0; j < num; j++)
        {
            bool val = ((b[pos + j / 8] >> (j % 8)) & 1) == 1;
            result.Add(new IniEntry { Hash = keys[j], Value = val ? 1 : 0 });
        }
        pos += byteCount;
        return result;
    }

    private static List<IniEntry> ReadNumbers(byte[] b, ref int pos, int bit)
    {
        // Maps bit index → (elementSize, count, mul)
        (int size, int count, double mul) = bit switch
        {
            0  => (4, 1, 1.0),   // int32
            1  => (4, 1, 1.0),   // float32
            2  => (1, 1, 0.1),   // uint8 * 0.1
            3  => (2, 1, 1.0),   // int16
            4  => (1, 1, 1.0),   // uint8
            6  => (1, 3, 0.1),   // uint8[3] * 0.1
            7  => (4, 3, 1.0),   // float[3]
            8  => (1, 2, 0.1),   // uint8[2] * 0.1
            9  => (4, 2, 1.0),   // float[2]
            10 => (1, 4, 0.1),   // uint8[4] * 0.1  (colors)
            11 => (4, 4, 1.0),   // float[4]
            13 => (8, 1, 1.0),   // int64
            _  => (0, 0, 0.0)
        };
        bool isFloat = bit is 1 or 7 or 9 or 11;

        var result = new List<IniEntry>();
        ushort num = ReadU16(b, ref pos);
        var keys   = new uint[num];
        for (int i = 0; i < num; i++) keys[i] = ReadU32(b, ref pos);

        for (int j = 0; j < num; j++)
        {
            if (count == 1)
            {
                double v = isFloat
                    ? BitConverter.ToSingle(b, pos) * mul
                    : ReadSignedInt(b, pos, size) * mul;
                pos += size;
                result.Add(new IniEntry { Hash = keys[j], Value = (count == 1 && mul == 1.0) ? (object)v : FormatFloat(v) });
            }
            else
            {
                var arr = new string[count];
                for (int k = 0; k < count; k++)
                {
                    double v = isFloat
                        ? BitConverter.ToSingle(b, pos) * mul
                        : ReadSignedInt(b, pos, size) * mul;
                    pos += size;
                    arr[k] = FormatFloat(v);
                }
                result.Add(new IniEntry { Hash = keys[j], Value = string.Join(" ", arr) });
            }
        }
        return result;
    }

    private static List<IniEntry> ReadStrings(byte[] b, ref int pos, int stringsLength)
    {
        var result = new List<IniEntry>();
        ushort num = ReadU16(b, ref pos);
        var keys   = new uint[num];
        for (int i = 0; i < num; i++) keys[i] = ReadU32(b, ref pos);
        var offsets = new ushort[num];
        for (int i = 0; i < num; i++) offsets[i] = ReadU16(b, ref pos);

        int dataStart = pos;
        for (int i = 0; i < num; i++)
        {
            int o = dataStart + offsets[i];
            var sb = new StringBuilder();
            while (o < b.Length && b[o] != 0) sb.Append((char)b[o++]);
            result.Add(new IniEntry
            {
                Hash                 = keys[i],
                Value                = sb.ToString(),
                OriginalStringOffset = offsets[i]   // store offset for patching
            });
        }
        pos += stringsLength;
        return result;
    }

    // ── Particle builder ─────────────────────────────────────────────────────

    /// <summary>
    /// Rebuilds particles from IniEntry list.
    /// The "System" section contains GroupPartN entries that name the emitter groups.
    /// Each emitter group has its own properties.
    /// We expose each emitter as a ParticleData with:
    ///   - Name = emitter/group name
    ///   - Strings = all resolved "key=value" pairs for display/editing
    /// </summary>
    private static List<ParticleData> BuildParticles(List<IniEntry> entries, Dictionary<uint, string> hashMap)
    {
        // Group entries by resolved section name
        var groups = new Dictionary<string, List<IniEntry>>(StringComparer.Ordinal);

        foreach (var e in entries)
        {
            string group = e.ResolvedSection ?? "Unknown";
            if (!groups.TryGetValue(group, out var list))
            { list = new List<IniEntry>(); groups[group] = list; }
            list.Add(e);
        }

        // Order: System first, then emitter groups, Unknown last
        var ordered = groups.Keys
            .OrderBy(k => k == "System" ? 0 : k == "Unknown" ? 2 : 1)
            .ThenBy(k => k)
            .ToList();

        var particles = new List<ParticleData>();
        foreach (var key in ordered)
        {
            var p = new ParticleData { Name = key };
            foreach (var e in groups[key])
            {
                string label = e.ResolvedName ?? $"hash:{e.Hash:X8}";
                string val   = FormatEntryValue(e.Value);
                p.Strings.Add($"{label}={val}");
            }
            particles.Add(p);
        }

        if (particles.Count == 0)
        {
            var p = new ParticleData { Name = "Particle" };
            foreach (var e in entries)
                p.Strings.Add($"{(e.ResolvedName ?? $"hash:{e.Hash:X8}")}={FormatEntryValue(e.Value)}");
            particles.Add(p);
        }

        return particles;
    }

    // ── Save / round-trip ────────────────────────────────────────────────────

    private static byte[] BuildPatched(TroybinDocument doc)
    {
        // For now: if the user edited string values, patch them in the original bytes.
        // Full re-serialization is complex (requires re-hashing + re-encoding all types).
        // We do in-place string patching: replace string values of the same length.

        if (doc.OriginalBytes.Length < 4)
            throw new InvalidOperationException("No original bytes to patch.");

        if (doc.Version == 2)
            return PatchV2Strings(doc);

        // For V1 or unknown: return original unchanged
        return doc.OriginalBytes;
    }

    private static byte[] PatchV2Strings(TroybinDocument doc)
    {
        // ── Step 1: Sync AllEntries string values from the current ParticleData ──
        // Build a lookup: hash → new string value from the edited particles
        var updatedStrings = new Dictionary<uint, string>();
        foreach (var entry in doc.AllEntries.Where(e => e.Value is string && e.ResolvedName != null))
        {
            // Find the matching value in the edited particles
            string? newVal = FindUpdatedValue(doc, entry);
            if (newVal != null)
                updatedStrings[entry.Hash] = newVal;
        }

        // Apply updated values back into AllEntries
        foreach (var entry in doc.AllEntries.Where(e => e.Value is string))
        {
            if (updatedStrings.TryGetValue(entry.Hash, out var updated))
                entry.Value = updated;
        }

        // ── Step 2: Check if any string changed length ─────────────────────────
        var b    = (byte[])doc.OriginalBytes.Clone();
        int pos  = 1;
        ushort stringsLength = ReadU16(b, ref pos);
        ushort flags         = ReadU16(b, ref pos);
        if (flags == 0) flags = ReadU16(b, ref pos);

        if ((flags & (1 << 12)) == 0) return b; // no string block

        // Skip all blocks before bit 12
        int scanPos = pos;
        for (int bit = 0; bit < 12; bit++)
        {
            if ((flags & (1 << bit)) == 0) continue;
            if (bit == 5) SkipBools(b, ref scanPos);
            else          SkipNumbers(b, ref scanPos, bit);
        }

        // Now at the string block header
        int strBlockStart = scanPos;
        ushort num        = ReadU16(b, ref scanPos);
        uint[] hashes     = new uint[num];
        for (int i = 0; i < num; i++) hashes[i] = ReadU32(b, ref scanPos);
        ushort[] offsets  = new ushort[num];
        for (int i = 0; i < num; i++) offsets[i] = ReadU16(b, ref scanPos);
        int dataStart     = scanPos;

        // Read all current string values from original bytes
        var currentStrings = new string[num];
        for (int i = 0; i < num; i++)
        {
            int o = dataStart + offsets[i];
            var sb = new StringBuilder();
            while (o < b.Length && b[o] != 0) sb.Append((char)b[o++]);
            currentStrings[i] = sb.ToString();
        }

        // Resolve new values for each slot
        var newStrings = new string[num];
        for (int i = 0; i < num; i++)
        {
            var entry = doc.AllEntries.FirstOrDefault(e => e.Hash == hashes[i] && e.Value is string);
            newStrings[i] = entry?.Value as string ?? currentStrings[i];
        }

        // Check if any length differs
        bool needsRebuild = false;
        for (int i = 0; i < num; i++)
            if (Encoding.ASCII.GetByteCount(newStrings[i]) != Encoding.ASCII.GetByteCount(currentStrings[i]))
            { needsRebuild = true; break; }

        if (!needsRebuild)
        {
            // ── In-place patch (same-length strings) ──
            for (int i = 0; i < num; i++)
            {
                if (newStrings[i] == currentStrings[i]) continue;
                int o = dataStart + offsets[i];
                var encoded = Encoding.ASCII.GetBytes(newStrings[i]);
                encoded.CopyTo(b, o);
            }
            return b;
        }

        // ── Full string-pool rebuild ───────────────────────────────────────────
        // Build new string data pool
        var newOffsets  = new ushort[num];
        var poolBuilder = new List<byte>();
        for (int i = 0; i < num; i++)
        {
            newOffsets[i] = (ushort)poolBuilder.Count;
            poolBuilder.AddRange(Encoding.ASCII.GetBytes(newStrings[i]));
            poolBuilder.Add(0); // null terminator
        }
        byte[] newPool = poolBuilder.ToArray();
        ushort newStrLen = (ushort)newPool.Length;

        // Build the new string block bytes
        var blockBytes = new List<byte>();
        blockBytes.AddRange(BitConverter.GetBytes(num));                         // count (u16)
        foreach (var h in hashes)  blockBytes.AddRange(BitConverter.GetBytes(h));   // hashes
        foreach (var o in newOffsets) blockBytes.AddRange(BitConverter.GetBytes(o)); // offsets
        blockBytes.AddRange(newPool);                                             // string data

        int oldBlockSize = 2 + num * 4 + num * 2 + stringsLength; // header + hashes + offsets + data
        int newBlockSize = blockBytes.Count;

        // Rebuild entire file buffer with the replaced block
        var result = new List<byte>();
        // Bytes before string block
        result.AddRange(b.Take(strBlockStart));
        // New string block
        result.AddRange(blockBytes);
        // Bytes after old string block
        result.AddRange(b.Skip(strBlockStart + oldBlockSize));

        // Patch stringsLength at offset 1 (u16)
        var resultArr = result.ToArray();
        var newStrLenBytes = BitConverter.GetBytes(newStrLen);
        resultArr[1] = newStrLenBytes[0];
        resultArr[2] = newStrLenBytes[1];

        return resultArr;
    }

    /// <summary>
    /// Looks up the current (edited) value for an AllEntries string entry
    /// by matching section+field against the current particle strings.
    /// </summary>
    private static string? FindUpdatedValue(TroybinDocument doc, IniEntry entry)
    {
        if (entry.ResolvedSection == null || entry.ResolvedName == null) return null;

        var particle = doc.Particles.FirstOrDefault(p =>
            string.Equals(p.Name, entry.ResolvedSection, StringComparison.OrdinalIgnoreCase));
        if (particle == null) return null;

        foreach (var s in particle.Strings)
        {
            int eq = s.IndexOf('=');
            if (eq <= 0) continue;
            var key = s.Substring(0, eq).Trim();
            if (string.Equals(key, entry.ResolvedName, StringComparison.OrdinalIgnoreCase))
                return s.Substring(eq + 1);
        }
        return null;
    }

    // ── Low-level read helpers ────────────────────────────────────────────────

    private static ushort ReadU16(byte[] b, ref int pos)
    { var v = BitConverter.ToUInt16(b, pos); pos += 2; return v; }

    private static uint ReadU32(byte[] b, ref int pos)
    { var v = BitConverter.ToUInt32(b, pos); pos += 4; return v; }

    private static long ReadSignedInt(byte[] b, int pos, int size) => size switch
    {
        1 => b[pos],
        2 => BitConverter.ToInt16(b, pos),
        4 => BitConverter.ToInt32(b, pos),
        8 => BitConverter.ToInt64(b, pos),
        _ => 0
    };

    private static void SkipBools(byte[] b, ref int pos)
    {
        ushort num = ReadU16(b, ref pos);
        pos += num * 4; // hashes
        pos += (num / 8) + (num % 8 > 0 ? 1 : 0); // packed bools
    }

    private static void SkipNumbers(byte[] b, ref int pos, int bit)
    {
        // elementSize (bytes per scalar) and elementCount (scalars per entry)
        int elemSize = bit switch { 1 => 4, 3 => 2, 4 or 2 => 1, 7 => 4, 9 => 4, 11 => 4, 6 => 1, 8 => 1, 10 => 1, 13 => 8, _ => 4 };
        int elemCount = bit switch { 6 => 3, 7 => 3, 8 => 2, 9 => 2, 10 => 4, 11 => 4, _ => 1 };
        ushort num = ReadU16(b, ref pos);
        pos += num * 4;                     // hashes
        pos += num * elemSize * elemCount;  // values
    }

    private static string ReadNullTerminated(byte[] b, int pos)
    {
        var sb = new StringBuilder();
        while (pos < b.Length && b[pos] != 0) sb.Append((char)b[pos++]);
        return sb.ToString();
    }

    private static string FormatFloat(double v)
    {
        if (v == Math.Floor(v)) return ((long)v).ToString();
        return v.ToString("G6", System.Globalization.CultureInfo.InvariantCulture);
    }

    private static string FormatEntryValue(object? v) => v switch
    {
        string s => s,
        double d => FormatFloat(d),
        float f  => FormatFloat(f),
        int i    => i.ToString(),
        bool bo  => bo ? "true" : "false",
        _        => v?.ToString() ?? ""
    };

    private static object SanitizeString(string s)
    {
        if (s == "true")  return 1;
        if (s == "false") return 0;
        if (float.TryParse(s, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out float f)) return (double)f;
        return s;
    }
}

// ── Supporting types ─────────────────────────────────────────────────────────

/// <summary>One decoded key-value entry from the inibin binary data.</summary>
public class IniEntry
{
    /// <summary>ihash of the full field path (section * field).</summary>
    public uint Hash { get; set; }

    /// <summary>Decoded value (string, double, int, or string for vectors).</summary>
    public object? Value { get; set; }

    /// <summary>Resolved human-readable field name (e.g. "p-texture"), null if unknown.</summary>
    public string? ResolvedName { get; set; }

    /// <summary>The section this entry belongs to (e.g. "GroupPart0"), null if not resolved.</summary>
    public string? ResolvedSection { get; set; }

    /// <summary>Byte offset of the string value in the original file (for in-place patching).</summary>
    public int OriginalStringOffset { get; set; } = -1;
}
