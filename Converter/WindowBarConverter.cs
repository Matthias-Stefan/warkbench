using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace warkbench.Converter;

public sealed class WindowBarConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
            return null;

        var firstChar = char.ToUpperInvariant(text[0]);
        if (firstChar < 'A' || firstChar > 'Z')
            return null;

        var uri = $"{BaseUri}project_name_{char.ToLowerInvariant(firstChar)}.svg";
        return new Uri(uri);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
    
    private const string BaseUri = "avares://warkbench/Assets/Icons/App/";
}