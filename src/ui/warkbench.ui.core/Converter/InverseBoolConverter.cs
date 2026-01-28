using Avalonia.Data.Converters;
using System.Globalization;

namespace warkbench.src.ui.core.Converter;

public class InverseBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return false;
        }
        
        return !boolValue;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            return false;
        }
        
        return !boolValue;
    }
}