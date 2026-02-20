namespace TroybinEditor.Utilities;

/// <summary>
/// Utility class for common operations.
/// </summary>
public static class ValidationHelper
{
    /// <summary>
    /// Validates if a string is a valid float.
    /// </summary>
    public static bool TryParseFloat(string? value, out float result)
    {
        return float.TryParse(value, out result);
    }
    
    /// <summary>
    /// Validates if a string is a valid integer.
    /// </summary>
    public static bool TryParseInt(string? value, out int result)
    {
        return int.TryParse(value, out result);
    }
    
    /// <summary>
    /// Clamps an integer value between min and max.
    /// </summary>
    public static int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
    
    /// <summary>
    /// Clamps a float value between min and max.
    /// </summary>
    public static float Clamp(float value, float min, float max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}

