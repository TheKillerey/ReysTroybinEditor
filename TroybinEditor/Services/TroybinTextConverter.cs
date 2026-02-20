namespace TroybinEditor.Services;

using System.IO;
using System.Text;
using TroybinEditor.Models;

/// <summary>
/// Converts TroybinDocument to/from a human-readable text (INI-like) format.
/// Format:
///   [EmitterName]
///   key=value
///   key=value
///
///   [NextEmitter]
///   ...
/// </summary>
public static class TroybinTextConverter
{
    private const string Header = "# Rey's Troybin Editor - Text Export\n# Format: [EmitterName] followed by key=value pairs\n# Edit values, then import back to .troybin\n\n";

    public static string ToText(TroybinDocument doc)
    {
        var sb = new StringBuilder();
        sb.Append(Header);

        foreach (var p in doc.Particles)
        {
            sb.AppendLine($"[{p.Name}]");
            foreach (var entry in p.Strings)
                sb.AppendLine(entry);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static async Task ExportToFileAsync(TroybinDocument doc, string filePath)
    {
        var text = ToText(doc);
        await File.WriteAllTextAsync(filePath, text, Encoding.UTF8);
    }

    /// <summary>
    /// Imports text back and updates the document's particle strings.
    /// Does NOT re-encode binary – use SaveFileAsync after this.
    /// </summary>
    public static TroybinDocument FromText(string text, TroybinDocument original)
    {
        var doc = original.Clone();
        doc.Particles.Clear();

        var lines = text.Split('\n')
            .Select(l => l.TrimEnd('\r'))
            .ToList();

        ParticleData? current = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (line.StartsWith("#") || line.Length == 0)
            {
                if (line.Length == 0 && current != null)
                {
                    doc.Particles.Add(current);
                    current = null;
                }
                continue;
            }

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                if (current != null)
                    doc.Particles.Add(current);
                current = new ParticleData { Name = line.Substring(1, line.Length - 2) };
                continue;
            }

            current?.Strings.Add(line);
        }

        if (current != null)
            doc.Particles.Add(current);

        doc.AllStrings = doc.Particles.SelectMany(p => p.Strings).ToList();
        doc.IsModified = true;
        return doc;
    }

    public static async Task<TroybinDocument> ImportFromFileAsync(string filePath, TroybinDocument original)
    {
        var text = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
        return FromText(text, original);
    }
}

