using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using warkbench.Models;
using warkbench.ViewModels;

namespace warkbench.Converter;

public class TreeNodeToPathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TreeNodeViewModel node)
        {
            return string.Empty;
        }

#if false
        if (node.Data is ResourceViewModel viewModel)
        {
            return viewModel.VirtualPath;
        }
#endif

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
