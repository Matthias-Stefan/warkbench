using Avalonia;
using Avalonia.Controls;
using warkbench.src.ui.viewport.Rendering;

namespace warkbench.src.ui.viewport.Controls;

public partial class WorldViewport : Control
{
    static WorldViewport()
    {
        SelectedItemsProperty.Changed.AddClassHandler<WorldViewport>((vp, _) => vp.SyncFromSelectedItems());
        SelectedItemProperty.Changed.AddClassHandler<WorldViewport>((vp, _) => vp.SyncFromSelectedItem());
        ActiveToolProperty.Changed.AddClassHandler<WorldViewport>((vp, _) => vp.OnActiveToolChanged());
        RenderablesProperty.Changed.AddClassHandler<WorldViewport>((vp, _) => vp.InvalidateVisual());
    }
    
    private void SyncFromSelectedItems()
    {
        if (_syncingSelection)
        {
            return;    
        }
        _syncingSelection = true;

        var items = SelectedItems ?? [];
        var primary = items.Count > 0 ? items[0] : null;

        if (!ReferenceEquals(SelectedItem, primary))
        {
            SelectedItem = primary;    
        }
        
        _syncingSelection = false;
    }
    
    private void SyncFromSelectedItem()
    {
        if (_syncingSelection)
        {
            return;
        }
        
        _syncingSelection = true;

        var item = SelectedItem;

        if (item is null)
        {
            if (SelectedItems.Count != 0)
            {
                SelectedItems = [];    
            }
        }
        else
        {
            if (SelectedItems.Count != 1 || !ReferenceEquals(SelectedItems[0], item))
            {
                SelectedItems = [item];    
            }
        }

        _syncingSelection = false;
    }
    
    private void OnActiveToolChanged()
    {
        // TODO:
        
        InvalidateVisual();
    }
    
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<WorldViewport, object?>(nameof(SelectedItem));
    
    public static readonly StyledProperty<IReadOnlyList<object>> SelectedItemsProperty =
        AvaloniaProperty.Register<WorldViewport, IReadOnlyList<object>>(nameof(SelectedItems), Array.Empty<object>());

    public static readonly StyledProperty<ViewportTool> ActiveToolProperty =
        AvaloniaProperty.Register<WorldViewport, ViewportTool>(nameof(ActiveTool), ViewportTool.None);
    
    public static readonly StyledProperty<IReadOnlyList<IRenderable>> RenderablesProperty =
        AvaloniaProperty.Register<WorldViewport, IReadOnlyList<IRenderable>>(nameof(Renderables), Array.Empty<IRenderable>());
    
    
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }
    
    public IReadOnlyList<object> SelectedItems
    {
        get => GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value ?? []);
    }
    
    public ViewportTool ActiveTool
    {
        get => GetValue(ActiveToolProperty);
        set => SetValue(ActiveToolProperty, value);
    }
    
    public IReadOnlyList<IRenderable> Renderables
    {
        get => GetValue(RenderablesProperty);
        set => SetValue(RenderablesProperty, value ?? []);
    }
    
    private bool _syncingSelection;
}