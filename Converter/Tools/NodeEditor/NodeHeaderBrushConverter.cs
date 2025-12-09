using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia;
using System.Globalization;
using System;
using warkbench.Brushes;


namespace warkbench.Converter;
public class NodeHeaderBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NodeHeaderBrushType nodeHeaderBrushType)
        {
            return new SolidColorBrush(Colors.Black);
        }

        if (nodeHeaderBrushType == NodeHeaderBrushType.None &&
            Application.Current is { } app && app.TryFindResource("Node.HeaderBrush", out var res) &&
            res is IBrush brush)
        {
            return brush;
        }

        return nodeHeaderBrushType switch
        {
            NodeHeaderBrushType.Blueprint => NodeBrushes.BlueprintBackground,
            NodeHeaderBrushType.Property  => NodeBrushes.Property,
            _ => throw new ArgumentOutOfRangeException(nameof(nodeHeaderBrushType))
        };
    }


    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
