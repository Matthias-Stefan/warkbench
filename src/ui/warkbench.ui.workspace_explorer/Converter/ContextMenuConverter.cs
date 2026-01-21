using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;
using warkbench.src.editors.workspace_explorer.ViewModels;

namespace warkbench.src.ui.workspace_explorer.Converter;

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

        if (value0 is null || value1 is not WorkspaceExplorerViewModel workspaceExplorerViewModel)
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
#if false
            case IProject project:
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Add package item",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_add_package_item")! },
                    Command = workspaceExplorerViewModel.AddWorld,
                    CommandParameter = packageViewModel
                });
#endif
            
            default:
                contextMenu.IsVisible = false;
                break;
        }

        return contextMenu;
    }
}