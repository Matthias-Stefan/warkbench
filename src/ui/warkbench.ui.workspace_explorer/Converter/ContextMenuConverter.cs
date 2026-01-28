using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;
using Avalonia.Media;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.editors.workspace_explorer.ViewModels;
using warkbench.src.ui.core.Themes;

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
            case IProject project:
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Save",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_save")! },
                    Command = workspaceExplorerViewModel.SaveCommand,
                    CommandParameter = project
                });
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Rename",
                    Command = workspaceExplorerViewModel.BeginRenameCommand,
                    CommandParameter = project
                });
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Close",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_close")! },
                    Command = workspaceExplorerViewModel.SaveCommand,
                    CommandParameter = project
                });
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Delete",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_delete")! },
                    Command = workspaceExplorerViewModel.SaveCommand,
                    CommandParameter = project
                });

                contextMenu.Items.Add(new Separator());
                
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Copy Path",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_content_copy")! },
                    Command = workspaceExplorerViewModel.SaveCommand,
                    CommandParameter = project
                });
                contextMenu.Items.Add(new MenuItem
                {
                    Header = "Open Containing Folder",
                    Icon = new PathIcon { Data = (Geometry)Application.Current.FindResource("icon_open_in_new")! },
                    Command = workspaceExplorerViewModel.SaveCommand,
                    CommandParameter = project
                });
                
                break;
            
            default:
                contextMenu.IsVisible = false;
                break;
        }

        return contextMenu;
    }
}