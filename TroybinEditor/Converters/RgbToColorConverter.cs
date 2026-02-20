namespace TroybinEditor.Converters;

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>
/// Converts RGBA values to a WPF Color for preview.
/// </summary>
public class RgbToColorConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Length < 4 || values.Any(v => v == null))
            return Colors.White;
        
        if (int.TryParse(values[0]?.ToString(), out var r) &&
            int.TryParse(values[1]?.ToString(), out var g) &&
            int.TryParse(values[2]?.ToString(), out var b) &&
            int.TryParse(values[3]?.ToString(), out var a))
        {
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }
        
        return Colors.White;
    }
    
    public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

