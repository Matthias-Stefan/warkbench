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
            RootWorldViewModel or WorldViewModel => new SolidColorBrush(Colors.Orange),
            RootPackageViewModel or PackageViewModel or PackageItemViewModel => new SolidColorBrush(Colors.LightGreen),
            RootPackageBlueprintViewModel or BlueprintViewModel => new SolidColorBrush(Color.FromRgb(30, 144, 255)),
            RootPropertiesViewModel or PropertyViewModel => new SolidColorBrush(Colors.Crimson),
            
            _ => new SolidColorBrush(Colors.White)
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}