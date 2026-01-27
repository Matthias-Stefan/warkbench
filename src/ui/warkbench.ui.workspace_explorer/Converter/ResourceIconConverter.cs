using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.ui.workspace_explorer.Converter;

public class ResourceIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var resourceKey = value switch
        {
            IProject => "icon_ad",
            IWorld => "icon_globe",

            LocalPath p when p.Value.Contains(IWorldIoService.Extension)
                => "icon_globe",

            "Worlds"     => "icon_globe",
            "Packages"   => "icon_package",
            "Blueprints" => "icon_blueprint",
            "Properties" => "icon_precision_manufacturing",

            _ => string.Empty
        };

        return Application.Current?.FindResource(resourceKey)
               ?? AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}