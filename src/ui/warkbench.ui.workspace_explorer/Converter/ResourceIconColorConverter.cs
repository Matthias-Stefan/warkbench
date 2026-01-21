using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace warkbench.src.ui.workspace_explorer.Converter;

public class ResourceIconColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            switch (stringValue)
            {
                case "Worlds":
                    return new SolidColorBrush(Colors.Orange);
                case "Packages":
                    return new SolidColorBrush(Colors.LightGreen);
                case "Blueprints":
                    return new SolidColorBrush(Color.FromRgb(30, 144, 255));
                case "Properties":
                    return new SolidColorBrush(Colors.Crimson);
            }
        }

        return new SolidColorBrush(Colors.White);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}