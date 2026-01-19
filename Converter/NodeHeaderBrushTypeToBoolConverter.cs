using Avalonia.Data.Converters;
using System.Globalization;
using System;
using ExCSS;
using warkbench.Brushes;


namespace warkbench.Converter;

public class NodeHeaderBrushTypeToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NodeHeaderBrushType nodeHeaderBrushType)
        {
            return false;
        }

        return nodeHeaderBrushType == NodeHeaderBrushType.None;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}