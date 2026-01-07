using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia;
using System;
using warkbench.core;


namespace warkbench.viewport;
public partial class WorldViewport : Control
{
    public WorldViewport()
    {
        Focusable = true;
    }
    
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;
        context.FillRectangle(_background, bounds);
        
        if (ShowGrid)
        {
            _gridRenderer.Render(context, _viewportCamera, bounds);
        }
        
        if (ActiveTool == ViewportTool.Selection && _viewportInputState.IsDragging && !_gizmo2D.IsActive)
        {
            var minX = Math.Min(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.MousePos.X);
            var maxX = Math.Max(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.MousePos.X);
            
            var minY = Math.Min(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.MousePos.Y);
            var maxY = Math.Max(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.MousePos.Y);
            
            _selectionRenderer.Render(context, _viewportCamera, bounds, new Rect(new Point(minX, minY), new Size(maxX - minX, maxY - minY)));
        }

        _gridRenderer.RenderOrigin(context, _viewportCamera, bounds);
        
#if DEBUG
        _debugRenderer.Render(context, _viewportCamera, bounds, _viewportInputState.MousePos);        
#endif
        
        // Update Gizmo
        var worldToScreenMatrix = _viewportCamera.WorldToScreenMatrix(bounds.Size);
        var gizmoOrigin = worldToScreenMatrix.Transform(_gizmo2D.Origin);
        _gizmo2D.UpdateHover(gizmoOrigin, _viewportInputState.MousePos);
        _gizmo2D.Render(context, worldToScreenMatrix);
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        
        var p = e.GetCurrentPoint(this).Position;
        
        if (e.Properties.IsLeftButtonPressed)
        {
            var gizmoScreenPos = _viewportCamera.WorldToScreen(_gizmo2D.Origin, Bounds.Size);
            var consumed = _gizmo2D.OnPointerPressed(gizmoScreenPos, p);
            if (consumed)
            {
                _viewportInputState.LeftButtonPressedPos = p;
                e.Pointer.Capture(this);
                e.Handled = true;
                return;
            }
            _viewportInputState.LeftButtonPressedPos = p;
        }
        if (e.Properties.IsMiddleButtonPressed)
        {
            _viewportInputState.MiddleButtonPressedPos = p;
        }
        if (e.Properties.IsRightButtonPressed)
        {
            _viewportInputState.RightButtonPressedPos = p;
        }
        
        InvalidateVisual();
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var p = e.GetCurrentPoint(this).Position;
        _viewportInputState.MousePos = p;
        
        if (e.Properties.IsLeftButtonPressed)
        {
            if (!_viewportInputState.IsDragging)
            {
                var dx = _viewportInputState.MousePos.X - _viewportInputState.LeftButtonPressedPos.X;
                var dy = _viewportInputState.MousePos.Y - _viewportInputState.LeftButtonPressedPos.Y;

                const double th = 12;
                if (dx * dx + dy * dy >= th * th)
                {
                    _viewportInputState.IsDragging = true;
                }
            }
        }
        
        if (_gizmo2D.IsActive)
        {
            _gizmo2D.OnPointerMoved(
                _viewportInputState.LeftButtonPressedPos,
                _viewportInputState.MousePos,
                _viewportCamera.ScreenToWorldMatrix(Bounds.Size),
                _viewportCamera.WorldToScreenMatrix(Bounds.Size));
        }
        
        if (e.Properties.IsMiddleButtonPressed)
        {
            var delta = p - _viewportInputState.MiddleButtonPressedPos;
            _viewportCamera.PanByPixels(delta);
            _viewportInputState.MiddleButtonPressedPos = p;
        }

        if (e.Properties.IsRightButtonPressed)
        {
            
        }

        InvalidateVisual();
        e.Handled = true;
    }
    
    protected override void OnPointerReleased(Avalonia.Input.PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        var p = e.GetCurrentPoint(this).Position;
        
        switch (e.InitialPressMouseButton)
        {
            case MouseButton.Left:
                _viewportInputState.IsDragging = false;
                _viewportInputState.LeftButtonPressedPos = WarkbenchMath.ZeroPoint;
                break;

            case MouseButton.Middle:
                _viewportInputState.MiddleButtonPressedPos = WarkbenchMath.ZeroPoint;
                break;

            case MouseButton.Right:
                _viewportInputState.RightButtonPressedPos = WarkbenchMath.ZeroPoint;
                break;
        }

        _gizmo2D.OnPointerReleased();

        var stillPressed = e.GetCurrentPoint(this).Properties;
        if (stillPressed is { IsLeftButtonPressed: false, IsMiddleButtonPressed: false, IsRightButtonPressed: false })
        {
            e.Pointer.Capture(null);
        }

        InvalidateVisual();
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
            _gridRenderer.GridSpacing = System.Math.Max(2.0, value);
            InvalidateVisual();
        }
    }
     
    public bool ShowGrid { get; set; } = true;
    
    private readonly IBrush  _background = new SolidColorBrush(Color.Parse("#191a1c"));
    
    private readonly ViewportCamera _viewportCamera = new(); 
    private readonly ViewportInputState _viewportInputState = new();

    private readonly Gizmo2D _gizmo2D = new();
    
    private readonly GridRenderer _gridRenderer = new();
    private readonly SelectionRenderer _selectionRenderer = new();
#if DEBUG
    private readonly DebugRenderer _debugRenderer = new();
#endif
}