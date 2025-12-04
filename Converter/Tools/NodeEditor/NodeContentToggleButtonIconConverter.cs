using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;


namespace warkbench.Converter;
public class NodeContentToggleButtonIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var key = value is bool and true
            ? "icon_expand_content"
            : "icon_collapse_content";

        return Application.Current!.FindResource(key) as Geometry;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var key = value is bool and true
            ? "icon_collapse_content"
            : "icon_expand_content";

        return Application.Current!.FindResource(key) as Geometry;
    }
}
