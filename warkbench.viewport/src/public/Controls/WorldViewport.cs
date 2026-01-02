using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;


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

        if (ActiveTool == ViewportTool.Selection && _viewportInputState.IsDragging && !_gizmoActive)
        {
            var minX = Math.Min(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.PointerPos.X);
            var maxX = Math.Max(_viewportInputState.LeftButtonPressedPos.X, _viewportInputState.PointerPos.X);
            
            var minY = Math.Min(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.PointerPos.Y);
            var maxY = Math.Max(_viewportInputState.LeftButtonPressedPos.Y, _viewportInputState.PointerPos.Y);
            
            _selectionRenderer.Render(context, _viewportCamera, bounds, new Rect(new Point(minX, minY), new Size(maxX - minX, maxY - minY)));
        }

        _gridRenderer.RenderOrigin(context, _viewportCamera, bounds);
        
#if DEBUG
        _debugRenderer.Render(context, _viewportCamera, bounds, _viewportInputState.PointerPos);        
#endif
        var gizmoOrigin = _viewportCamera.WorldToScreen(_gizmoOrigin, Bounds.Size);
        _gizmo2D.UpdateHover(gizmoOrigin, _viewportInputState.PointerPos);
        _gizmo2D.Render(context, gizmoOrigin);
    }

    protected override void OnPointerPressed(Avalonia.Input.PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        Focus();
        
        var p = e.GetCurrentPoint(this).Position;

        if (e.Properties.IsLeftButtonPressed)
        {
            var gizmoScreenPos = _viewportCamera.WorldToScreen(_gizmoOrigin, Bounds.Size);
            if (_gizmo2D.OnPointerPressed(gizmoScreenPos, p))
            {
                _gizmoActive = true;
                _gizmoOriginPointerOffset = gizmoScreenPos - p;
                
                e.Pointer.Capture(this);
                e.Handled = true;
                return;
            }
        }

        _gizmoActive = false;
        
        if (e.Properties.IsLeftButtonPressed)
        {
            _viewportInputState.LeftButtonPressedPos = p;
            e.Pointer.Capture(this);
            e.Handled = true;
        }
        if (e.Properties.IsMiddleButtonPressed)
        {
            _viewportInputState.MiddleButtonPressedPos = p;
            e.Pointer.Capture(this);
            e.Handled = true;
        }
        if (e.Properties.IsRightButtonPressed)
        {
            _viewportInputState.RightButtonPressedPos = p;
            e.Pointer.Capture(this);
            e.Handled = true;
        }
    }

    protected override void OnPointerMoved(Avalonia.Input.PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var p = e.GetCurrentPoint(this).Position;
        _viewportInputState.PointerPos = p;
        
        if (e.Properties.IsLeftButtonPressed)
        {
            if (!_viewportInputState.IsDragging)
            {
                var dx = _viewportInputState.PointerPos.X - _viewportInputState.LeftButtonPressedPos.X;
                var dy = _viewportInputState.PointerPos.Y - _viewportInputState.LeftButtonPressedPos.Y;

                const double th = 12;
                if (dx * dx + dy * dy >= th * th)
                {
                    _viewportInputState.IsDragging = true;
                }
            }
        }

        if (_gizmoActive)
        {
            var dx = _viewportInputState.PointerPos.X - _viewportInputState.LeftButtonPressedPos.X + _gizmoOriginPointerOffset.X;
            var dy = _viewportInputState.PointerPos.Y - _viewportInputState.LeftButtonPressedPos.Y + _gizmoOriginPointerOffset.Y;
            
            var startScreen = _viewportInputState.LeftButtonPressedPos;

            switch (_gizmo2D.ActivePart)
            {
                case Gizmo2D.Part.Center:
                    var currentScreen = startScreen + new Vector(dx, dy);
                    var currentWorld = _viewportCamera.ScreenToWorld(currentScreen, Bounds.Size);
                    _gizmoOrigin = new Point(currentWorld.X, currentWorld.Y);
                    break;
                case Gizmo2D.Part.TranslateX:
                    currentScreen = startScreen + new Vector(dx, 0);
                    currentWorld = _viewportCamera.ScreenToWorld(currentScreen, Bounds.Size);
                    _gizmoOrigin = new Point(currentWorld.X, _gizmoOrigin.Y);
                    break;
                case Gizmo2D.Part.TranslateY:
                    currentScreen = startScreen + new Vector(0, dy);
                    currentWorld = _viewportCamera.ScreenToWorld(currentScreen, Bounds.Size);
                    _gizmoOrigin = new Point(_gizmoOrigin.X, currentWorld.Y);
                    break;
                
                case Gizmo2D.Part.ScaleX:
                    break;
                case Gizmo2D.Part.ScaleY:
                    break;
                case Gizmo2D.Part.Rotate:
                    break;
            }
            


            
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
                _gizmoActive = false;
                break;

            case MouseButton.Middle:
                _viewportInputState.MiddleButtonPressedPos = WarkbenchMath.ZeroPoint;
                break;

            case MouseButton.Right:
                _viewportInputState.RightButtonPressedPos = WarkbenchMath.ZeroPoint;
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
    
    private readonly IBrush  _background = new SolidColorBrush(Color.Parse("#191a1c"));
    
    private readonly ViewportCamera _viewportCamera = new(); 
    private readonly ViewportInputState _viewportInputState = new();

    private bool _gizmoActive = false;
    private Point _gizmoOrigin = WarkbenchMath.ZeroPoint;
    private Point _gizmoOriginPointerOffset = WarkbenchMath.ZeroPoint;
    private readonly Gizmo2D _gizmo2D = new();
    
    private readonly GridRenderer _gridRenderer = new();
    private readonly SelectionRenderer _selectionRenderer = new();
#if DEBUG
    private readonly DebugRenderer _debugRenderer = new();
#endif
}