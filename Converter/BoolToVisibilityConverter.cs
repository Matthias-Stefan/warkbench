using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ExCSS;


namespace warkbench.Converter;
public class BoolToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue == true
                ? Visibility.Visible
                : Visibility.Collapse;
        }
        
        return value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
