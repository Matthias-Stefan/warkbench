using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System.Collections.Specialized;
using System.Windows.Input;
using System;
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
        
        RootPropertyMenuNodeMenuRoot = CreateMainMenuItem(
            "Properties", 
            "icon_precision_manufacturing", 
            NodeBrushes.Property, 
            null);
        
        RootPropertyMenuNodePaletteMenu = CreateMainMenuItem(
            "Properties", 
            "icon_precision_manufacturing", 
            NodeBrushes.Property, 
            null);
        
        if (vm.ProjectManager.CurrentProject is not null)
        {
            vm.ProjectManager.CurrentProject.Properties.CollectionChanged += ProjectProperties_CollectionChanged;
            vm.ProjectManager.ProjectChanging += (o, args) =>
            {
                vm.ProjectManager.CurrentProject.Properties.CollectionChanged -= ProjectProperties_CollectionChanged;
            };
            vm.ProjectManager.ProjectChanged += (o, args) =>
            {
                vm.ProjectManager.CurrentProject.Properties.CollectionChanged += ProjectProperties_CollectionChanged;
            };
        }
        
        BuildMenus(vm);
    }
    
    private void BuildMenus(NodeEditorViewModel vm)
    {
        NodeMenuRoot.Items.Add(RootPropertyMenuNodeMenuRoot);
        NodeMenuRoot.Items.Add(CreateMainMenuItem("2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Int32", "icon_int32", NodeBrushes.Int32, vm.AddInt32NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Int64", "icon_int64", NodeBrushes.Int64, vm.AddInt64NodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("String", "icon_string", NodeBrushes.String, vm.AddStringNodeCommand));
        NodeMenuRoot.Items.Add(CreateMainMenuItem("Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeCommand));
       
        NodePaletteMenu.Items.Add(RootPropertyMenuNodePaletteMenu);
        NodePaletteMenu.Items.Add(CreateMainMenuItem("2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Int32", "icon_int32", NodeBrushes.Int32, vm.AddInt32NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Int64", "icon_int64", NodeBrushes.Int64, vm.AddInt64NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("String", "icon_string", NodeBrushes.String, vm.AddStringNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeFromMouseCommand));

        CreatePropertyMenu();
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

    private void CreatePropertyMenu()
    {
        if (DataContext is not NodeEditorViewModel vm)
        {
            return;    
        }

        foreach (var property in vm.ProjectManager.CurrentProject.Properties)
        {
            AddPropertyMenuEntry(RootPropertyMenuNodeMenuRoot, property, vm.AddPropertyNodeCommand);
            AddPropertyMenuEntry(RootPropertyMenuNodePaletteMenu, property, vm.AddPropertyNodeFromMouseCommand);
        }
    }

    private void AddPropertyMenuEntry(MenuItem menuItem, GraphModel graphModel, ICommand command)
    {
        menuItem.Items.Add(new MenuItem
        {
            Header = new TextBlock { Text = graphModel.Name },
            Command = command,
            CommandParameter = new Tuple<TransformGroup, GraphModel>(Editor.ViewportTransform as TransformGroup, graphModel),
            Icon = new PathIcon
            {
                Data = (Geometry)Application.Current!.FindResource("icon_property_node")!,
                Foreground = NodeBrushes.Property
            }
        });
    }

    private void ProjectProperties_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (DataContext is not NodeEditorViewModel vm)
        {
            return;    
        }
        
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems?[0] is not GraphModel graphModel)
            {
                return;
            }
            
            AddPropertyMenuEntry(RootPropertyMenuNodeMenuRoot, graphModel, vm.AddPropertyNodeCommand);
            AddPropertyMenuEntry(RootPropertyMenuNodePaletteMenu, graphModel, vm.AddPropertyNodeFromMouseCommand);
        }
    }

    private MenuItem RootPropertyMenuNodeMenuRoot { get; set; }
    private MenuItem RootPropertyMenuNodePaletteMenu { get; set; }
}