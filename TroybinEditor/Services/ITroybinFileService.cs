namespace TroybinEditor.Services;

using TroybinEditor.Models;

/// <summary>
/// Interface for Troybin file I/O operations.
/// </summary>
public interface ITroybinFileService
{
    /// <summary>
    /// Loads a Troybin file from the specified path.
    /// </summary>
    Task<TroybinDocument> LoadFileAsync(string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Saves a Troybin document to the specified path.
    /// </summary>
    Task SaveFileAsync(TroybinDocument document, string filePath, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates a Troybin document for consistency.
    /// </summary>
    bool ValidateDocument(TroybinDocument document);
}

