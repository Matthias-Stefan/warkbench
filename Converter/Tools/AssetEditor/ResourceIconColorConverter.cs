using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using warkbench.ViewModels;


namespace warkbench.Converter;
public class ResourceIconColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            RootPackageViewModel or PackageViewModel => new SolidColorBrush(Colors.LightGreen),
            RootPackageBlueprintViewModel or PackageBlueprintViewModel => new SolidColorBrush(Color.FromRgb(30, 144, 255)),
            RootPropertiesViewModel => new SolidColorBrush(Colors.Violet),
            
            PackageItemViewModel => new SolidColorBrush(Colors.LightSalmon),
            _ => new SolidColorBrush(Colors.White)
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
