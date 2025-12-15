using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System.Windows.Input;
using warkbench.Brushes;
using warkbench.Models;
using warkbench.ViewModels;


namespace warkbench.Views;
public partial class NodeEditorView : UserControl
{
    public NodeEditorView()
    {
        InitializeComponent();
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (DataContext is not NodeEditorViewModel vm)
            return;
        
        BuildMenus(vm);
    }
    
    private void BuildMenus(NodeEditorViewModel vm)
    {
        MenuItem CreatePropertyMenu(ICommand command)
        {
            var root = CreateMainMenuItem(
                "Properties",
                "icon_precision_manufacturing",
                NodeBrushes.Property,
                null);

            if (vm.ProjectManager.CurrentProject is null)
                return root;

            foreach (var property in vm.ProjectManager.CurrentProject.Properties)
            {
                root.Items.Add(new MenuItem
                {
                    Header = new TextBlock { Text = property.Name },
                    Command = command,
                    CommandParameter = new Tuple<TransformGroup, GraphModel>(Editor.ViewportTransform as TransformGroup, property),
                    Icon = new PathIcon
                    {
                        Data = (Geometry)Application.Current!
                            .FindResource("icon_property_node")!,
                        Foreground = NodeBrushes.Property
                    }
                });
            }

            return root;
        }
        
        NodeMenuRoot.Items.Add(CreatePropertyMenu(vm.AddPropertyNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Int32", "icon_int32", NodeBrushes.Int32, vm.AddInt32NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Int64", "icon_int64", NodeBrushes.Int64, vm.AddInt64NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("String", "icon_string", NodeBrushes.String, vm.AddStringNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeCommand));
       
        NodePaletteMenu.Items.Add(CreatePropertyMenu(vm.AddPropertyNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Int32", "icon_int32", NodeBrushes.Int32, vm.AddInt32NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Int64", "icon_int64", NodeBrushes.Int64, vm.AddInt64NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("String", "icon_string", NodeBrushes.String, vm.AddStringNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeFromMouseCommand));
    }

    private MenuItem CreateMainMenuItem(string header, string iconKey, SolidColorBrush color, ICommand? command)
    {
        return new MenuItem
        {
            Header = new TextBlock { Text = header },
            Command = command,
            CommandParameter = Editor.ViewportTransform,
            Icon = new PathIcon
            {
                Data = (Geometry)Application.Current!.FindResource(iconKey)!,
                Foreground = color,
            }
        };
    }
    
    private void NodifyEditor_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            if (Editor.ContextMenu is { } menu)
            {
                menu.PlacementTarget = Editor;

                menu.Open();
            }
        }
        else if (e.Key == Key.Delete)
        {
            if (DataContext is NodeEditorViewModel vm)
            {
                vm.RemoveNodeViewModel(Editor.SelectedItem as NodeViewModel);
            }
        }

        e.Handled = true;
    }

    private void NodifyEditor_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is NodeEditorViewModel vm)
        {
            vm.LastMousePosition = e.GetPosition(this);
        }
    }
}