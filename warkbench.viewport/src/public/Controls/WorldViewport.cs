using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;


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
        context.FillRectangle(new SolidColorBrush(Color.Parse("#191a1c")), bounds);

        if (ShowGrid)
        {
            _gridRenderer.Render(context, _viewportCamera, bounds);
        }

        if (_viewportInputState.IsDragging)
        {
            var minX = Math.Min(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.LeftButtonPos.X);
            var maxX = Math.Max(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.LeftButtonPos.X);
            
            var minY = Math.Min(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.LeftButtonPos.Y);
            var maxY = Math.Max(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.LeftButtonPos.Y);
            
            _selectionRenderer.Render(context, _viewportCamera, bounds, new Rect(new Point(minX, minY), new Size(maxX - minX, maxY - minY)));
        }

        _gridRenderer.RenderOriginGizmo(context, bounds, _viewportCamera);
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        
        var p = e.GetCurrentPoint(this).Position;
        if (e.Properties.IsLeftButtonPressed)
        {
            _viewportInputState.LeftButtonPressed(p);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
        if (e.Properties.IsMiddleButtonPressed)
        {
            _viewportInputState.MiddleButtonPressed(p);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
        if (e.Properties.IsRightButtonPressed)
        {
            _viewportInputState.RightButtonPressed(p);
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var p = e.GetCurrentPoint(this).Position;

        if (e.Properties.IsLeftButtonPressed)
        {
            _viewportInputState.LeftButtonPos = p;

            if (!_viewportInputState.IsDragging)
            {
                var dx = _viewportInputState.LeftButtonPos.X - _viewportInputState.LeftButtonPressedPos.X;
                var dy = _viewportInputState.LeftButtonPos.Y - _viewportInputState.LeftButtonPressedPos.Y;

                const double th = 12;
                if (dx * dx + dy * dy >= th * th)
                {
                    _viewportInputState.IsDragging = true;
                }
            }
        }

        if (e.Properties.IsMiddleButtonPressed)
        {
            var delta = p - _viewportInputState.MiddleButtonPos;
            _viewportInputState.MiddleButtonPos = p;
            _viewportCamera.PanByPixels(delta);
        }

        if (e.Properties.IsRightButtonPressed)
        {
            _viewportInputState.RightButtonPos = p;
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
                _viewportInputState.LeftButtonPos = p;
                _viewportInputState.IsDragging = false;
                _viewportInputState.LeftButtonReleased();
                break;

            case MouseButton.Middle:
                _viewportInputState.MiddleButtonPos = p;
                _viewportInputState.MiddleButtonReleased();
                break;

            case MouseButton.Right:
                _viewportInputState.RightButtonPos = p;
                _viewportInputState.RightButtonReleased();
                break;
        }

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
    
    private readonly ViewportCamera _viewportCamera = new(); 
    private readonly ViewportInputState _viewportInputState = new();
    
    private readonly GridRenderer _gridRenderer = new();
    private readonly SelectionRenderer _selectionRenderer = new();
}