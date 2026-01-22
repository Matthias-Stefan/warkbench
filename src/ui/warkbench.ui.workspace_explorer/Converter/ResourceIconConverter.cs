using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;

namespace warkbench.src.ui.workspace_explorer.Converter;

public class ResourceIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var resourceKey = string.Empty;
        if (value is string stringValue)
        {
            resourceKey = value switch
            {
                "Project" => "icon_ad",
                "Worlds" => "icon_globe",
                "Packages" => "icon_package",
                "Blueprints" => "icon_blueprint",
                "Properties" => "icon_precision_manufacturing",
                _ => string.Empty
            };
        }

        return Application.Current?.FindResource(resourceKey) ?? AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}