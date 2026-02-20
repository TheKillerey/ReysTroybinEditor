namespace TroybinEditor.Converters;

using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

/// <summary>Null → Collapsed, non-null → Visible</summary>
public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value == null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Non-null → Visible, null → Collapsed</summary>
public class NotNullToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value != null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>bool true → Visible, false → Collapsed</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Inspects a string value and returns a color-coded brush indicating its type:
/// color-vector → purple, .dds/.scb → teal, blend mode → orange, number → blue, other → gray
/// </summary>
public class ValueTypeToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush BrushVector  = new(Color.FromRgb(0x7C, 0x3A, 0xED)); // purple
    private static readonly SolidColorBrush BrushFile    = new(Color.FromRgb(0x06, 0x9D, 0x86)); // teal
    private static readonly SolidColorBrush BrushBlend   = new(Color.FromRgb(0xD9, 0x77, 0x06)); // orange
    private static readonly SolidColorBrush BrushNumber  = new(Color.FromRgb(0x25, 0x63, 0xEB)); // blue
    private static readonly SolidColorBrush BrushDefault = new(Color.FromRgb(0x47, 0x55, 0x69)); // slate

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s) return BrushDefault;
        return ClassifyString(s);
    }

    internal static SolidColorBrush ClassifyString(string s)
    {
        if (s.EndsWith(".dds", StringComparison.OrdinalIgnoreCase) ||
            s.EndsWith(".scb", StringComparison.OrdinalIgnoreCase) ||
            s.EndsWith(".sco", StringComparison.OrdinalIgnoreCase))
            return BrushFile;

        if (s is "Simple" or "Complex" or "Add" or "Screen" or "Multiply")
            return BrushBlend;

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3 && parts.All(p => float.TryParse(p,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _)))
            return BrushVector;

        if (parts.Length == 1 && float.TryParse(s,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            return BrushNumber;

        return BrushDefault;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Inspects a string value and returns a short type label.</summary>
public class ValueTypeToLabelConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s) return "str";

        if (s.EndsWith(".dds", StringComparison.OrdinalIgnoreCase)) return "texture";
        if (s.EndsWith(".scb", StringComparison.OrdinalIgnoreCase) ||
            s.EndsWith(".sco", StringComparison.OrdinalIgnoreCase)) return "mesh";
        if (s is "Simple" or "Complex" or "Add" or "Screen" or "Multiply") return "blend";

        var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3 && parts.All(p => float.TryParse(p,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _)))
            return parts.Length == 4 ? "vec4" : parts.Length == 5 ? "color" : "vec";

        if (float.TryParse(s, System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            return "number";

        return "string";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
