using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia;
using System.Globalization;
using System;
using warkbench.Brushes;


namespace warkbench.Converter;
public class NodeIsBlueprintConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return new SolidColorBrush(Colors.Black);    
        }

        if (!boolValue && (Application.Current is { } app &&
                           app.TryFindResource("Node.HeaderBrush", out var res) &&
                           res is IBrush brush))
        {
            return brush;
        }

        return BlueprintBackgroundBrush.Get();
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
