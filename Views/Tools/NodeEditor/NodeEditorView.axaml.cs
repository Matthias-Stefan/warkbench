using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System.Windows.Input;
using warkbench.Brushes;
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
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add 2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Int", "icon_int", NodeBrushes.Int, vm.AddIntNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add String", "icon_string", NodeBrushes.String, vm.AddStringNodeCommand));
        NodesMenuRoot.Items.Add(CreateMainMenuItem("Add Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeCommand));
        
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add 2D-Vector", "icon_vec2", NodeBrushes.Vector2D, vm.AddVec2NodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Bool", "icon_bool_node", NodeBrushes.Bool, vm.AddBoolNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Class", "icon_class", NodeBrushes.Class, vm.AddClassNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Float", "icon_float", NodeBrushes.Float, vm.AddFloatNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Int", "icon_int", NodeBrushes.Int, vm.AddIntNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Rectangle", "icon_rect_node", NodeBrushes.Rectangle, vm.AddRectNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add String", "icon_string", NodeBrushes.String, vm.AddStringNodeFromMouseCommand));
        NodePaletteMenu.Items.Add(CreateMainMenuItem("Add Texture", "icon_texture_node", NodeBrushes.Texture, vm.AddTextureNodeFromMouseCommand));
    }

    private MenuItem CreateMainMenuItem(string header, string iconKey, SolidColorBrush color, ICommand command)
    {
        return new MenuItem
        {
            Header = header,
            Command = command,
            CommandParameter = Editor.ViewportTransform,
            Icon = new PathIcon
            {
                Data = (Geometry)Application.Current.FindResource(iconKey)!,
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