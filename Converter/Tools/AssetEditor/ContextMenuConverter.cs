using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Media;
using warkbench.Models;
using warkbench.ViewModels;


namespace warkbench.Converter;
public class ContextMenuConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count != 2 || !targetType.IsAssignableFrom(typeof(ContextMenu)))
        {
            throw new NotSupportedException();
        }

        if (values.Any(v => v == AvaloniaProperty.UnsetValue))
        { 
            return null;
        }

        var value0 = values[0];
        var value1 = values[1];

        if (value0 is null || value1 is not AssetEditorViewModel assetEditorViewModel)
        {
            return null;
        }

        if (Application.Current is null)
        {
            return null;
        }
        
        var contextMenu = new ContextMenu();
        switch (value0)
        {
            case PackageViewModel packageViewModel:
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Add package item",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_add_package_item")! },
                    Command = assetEditorViewModel.AddPackageItemCommand,
                    CommandParameter = packageViewModel
                });
                
                contextMenu.Items.Add(new Separator());
                
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Remove",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_delete")! },
                    Command = assetEditorViewModel.RemovePackageCommand,
                    CommandParameter = packageViewModel
                });
                break;

            case BlueprintViewModel packageBlueprintViewModel:
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Remove",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_delete")! },
                    Command = assetEditorViewModel.RemovePackageBlueprintCommand,
                    CommandParameter = packageBlueprintViewModel
                });
                break;
            
            default:
                contextMenu.IsVisible = false;
                break;
        }

        return contextMenu;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}
