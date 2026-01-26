using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.ui.core.Themes;

namespace warkbench.src.ui.workspace_explorer.Converter;

public class ResourceIconColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            IProject => new SolidColorBrush(Colors.White),

            string s when s.Contains(IWorldIoService.Extension)
                => new SolidColorBrush(Colors.White),

            "Worlds"     => new SolidColorBrush(WarkbenchStyle.SunflowerGoldColor),
            "Packages"   => new SolidColorBrush(WarkbenchStyle.ElectricRoseColor),
            "Blueprints" => new SolidColorBrush(Color.FromRgb(30, 144, 255)),
            "Properties" => new SolidColorBrush(Colors.LawnGreen),

            _ => new SolidColorBrush(Colors.White)
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}