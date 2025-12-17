using System;
using Avalonia.Controls;
using Avalonia.Media;


namespace warkbench.viewport;
public class WorldViewport : Control
{
    public WorldViewport()
    {
        Focusable = true;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        context.FillRectangle(new SolidColorBrush(Color.Parse("#191a1c")), bounds);

        if (ShowGrid)
        {
            _gridRenderer.Render(context, bounds, _viewportCamera);
        }
        
        _gridRenderer.RenderOriginGizmo(context, bounds, _viewportCamera);
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        Focus();

        var p = e.GetCurrentPoint(this);
        if (!p.Properties.IsMiddleButtonPressed)
        {
            return;    
        }
        
        _viewportInputState.BeginPan(p.Position);
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_viewportInputState.IsPanning)
        {
            return;
        }

        var p = e.GetCurrentPoint(this).Position;
        var delta = p - _viewportInputState.LastPointerPosition;

        _viewportInputState.LastPointerPosition = p;

        _viewportCamera.PanByPixels(delta);

        InvalidateVisual();
        e.Handled = true;
    }

    protected override void OnPointerReleased(Avalonia.Input.PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!_viewportInputState.IsPanning)
        {
            return;    
        }

        _viewportInputState.EndPan();
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(Avalonia.Input.PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        var mouse = e.GetPosition(this);

        var wheel = e.Delta.Y;
        if (wheel == 0)
            return;

        var factor = wheel > 0 ? 1.1 : (1.0 / 1.1);

        _viewportCamera.ZoomAt(mouse, factor, Bounds.Size);

        InvalidateVisual();
        e.Handled = true;
    }

    public double GridSpacing
    {
        get => _gridRenderer.GridSpacing;
        set
        {
            _gridRenderer.GridSpacing = Math.Max(2.0, value);
            InvalidateVisual();
        }
    }
    
    public bool ShowGrid { get; set; } = true;
    
    private readonly ViewportCamera _viewportCamera = new(); 
    private readonly ViewportInputState _viewportInputState = new();
    private readonly GridRenderer _gridRenderer = new();
}