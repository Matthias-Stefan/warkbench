using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Globalization;
using warkbench.ViewModels;


namespace warkbench.Converter;
public class ResourceIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var resourceKey = value switch
        {
            RootPackageViewModel => "icon_package",
            PackageViewModel => "icon_package",
            
            RootPackageBlueprintViewModel => "icon_blueprint",
            PackageBlueprintViewModel => "icon_blueprint_package",
            
            RootPropertiesViewModel => "icon_precision_manufacturing",
            
            PackageItemViewModel => "icon_ev_shadow",
            _ => string.Empty
        };

        return Application.Current?.FindResource(resourceKey) ?? AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
