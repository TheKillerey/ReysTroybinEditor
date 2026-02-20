namespace TroybinEditor.Utilities;

using System.Windows.Media;

/// <summary>
/// Utility class for color operations.
/// </summary>
public static class ColorHelper
{
    /// <summary>
    /// Converts RGBA values to a WPF Color.
    /// </summary>
    public static Color FromRgba(int red, int green, int blue, int alpha)
    {
        return Color.FromArgb(
            (byte)Clamp(alpha, 0, 255),
            (byte)Clamp(red, 0, 255),
            (byte)Clamp(green, 0, 255),
            (byte)Clamp(blue, 0, 255)
        );
    }
    
    /// <summary>
    /// Converts a WPF Color to RGBA tuple.
    /// </summary>
    public static (int Red, int Green, int Blue, int Alpha) ToRgba(Color color)
    {
        return (color.R, color.G, color.B, color.A);
    }
    
    /// <summary>
    /// Converts RGB to Hex string.
    /// </summary>
    public static string ToHex(int red, int green, int blue)
    {
        return $"#{red:X2}{green:X2}{blue:X2}";
    }
    
    /// <summary>
    /// Converts Hex string to RGB.
    /// </summary>
    public static (int Red, int Green, int Blue) FromHex(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor))
            return (255, 255, 255);
        
        hexColor = hexColor.TrimStart('#');
        
        if (hexColor.Length == 6 && int.TryParse(hexColor, System.Globalization.NumberStyles.HexNumber, null, out var rgb))
        {
            return (
                (rgb >> 16) & 0xFF,
                (rgb >> 8) & 0xFF,
                rgb & 0xFF
            );
        }
        
        return (255, 255, 255);
    }
    
    /// <summary>
    /// Clamps a value between 0 and 255.
    /// </summary>
    private static int Clamp(int value, int min, int max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}

