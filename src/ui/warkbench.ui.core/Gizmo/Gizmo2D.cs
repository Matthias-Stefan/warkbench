using Avalonia.Threading;
using Avalonia;
using System.Collections.Generic;
using System.Linq;
using System;
using warkbench.src.ui.core.Math;


namespace warkbench.src.ui.core.Gizmo;

public partial class Gizmo2D : IDisposable
{
    internal enum Part
    {
        None,
        Center,
        TranslateX,
        TranslateY,
        ScaleX,
        ScaleY,
        Rotate
    }

    public Gizmo2D()
    {
        _toolTipTimer.Tick += OnTooltipTimerTick;
    }

    public void Dispose()
    {
        _toolTipTimer.Stop();
        _toolTipTimer.Tick -= OnTooltipTimerTick;
    }

    public void UpdateHover(Point origin, Point mousePos)
    {
        HoveredPart = HitTest(origin, mousePos);

        var ringRadiusScreenSpace = MenuOffset + RotateRadius + IndicatorLength;

        foreach (var menuButton in _menuButtons)
        {
            menuButton.UpdateHover(mousePos, origin, ringRadiusScreenSpace, MenuButtonHitRadius);

            if (menuButton is { IsHovered: true, ShouldShowTooltip: false })
                _toolTipTimer.Start();
        }

        if (!_menuButtons.Any(b => b.IsHovered))
            _toolTipTimer.Stop();
    }

    public bool OnPointerPressed(Point origin, Point mousePos)
    {
        var ringRadiusScreenSpace = MenuOffset + RotateRadius + IndicatorLength;

        foreach (var menuButton in _menuButtons)
            menuButton.IsPressed = menuButton.HitTest(mousePos, origin, ringRadiusScreenSpace, MenuButtonHitRadius);

        var hit = HitTest(origin, mousePos);
        if (hit == Part.None)
            return false;

        ActivePart = hit;

        // store the initial pointer-to-origin delta (screen space) for stable dragging
        OriginPointerOffset = origin - mousePos;

        // incremental drag integration state (prevents jumps when modifier band changes)
        _lastDragMousePosScreen = mousePos;
        _dragAccumulatedScreenOffset = AvaloniaMathExtension.ZeroPoint;

        if (hit == Part.Rotate)
        {
            var startVec = mousePos - origin;
            RotateStartPointerAngle = AvaloniaMathExtension.Atan2(startVec);
            RotateStartGizmoAngle   = Angle;
        }

        return true;
    }

    public void OnPointerMoved(
        Point leftButtonPressedPos,
        Point mousePos,
        Matrix screenToWorld,
        Matrix worldToScreen)
    {
        if (!IsActive)
            return;

        switch (ActivePart)
        {
            case Part.Center:
                DragCenter(leftButtonPressedPos, mousePos, screenToWorld);
                return;

            case Part.TranslateX:
                DragTranslateX(leftButtonPressedPos, mousePos, screenToWorld, worldToScreen);
                return;

            case Part.TranslateY:
                DragTranslateY(leftButtonPressedPos, mousePos, screenToWorld, worldToScreen);
                return;

            case Part.ScaleX:
                DragScaleX(mousePos, worldToScreen);
                return;

            case Part.ScaleY:
                DragScaleY(mousePos, worldToScreen);
                return;

            case Part.Rotate:
                DragRotate(mousePos, worldToScreen);
                return;
        }
    }

    public void OnPointerReleased()
    {
        foreach (var menuButton in _menuButtons)
            menuButton.IsPressed = false;

        ActivePart = Part.None;

        ScaleXVisualLen = 0d;
        ScaleYVisualLen = 0d;

        _dragAccumulatedScreenOffset = AvaloniaMathExtension.ZeroPoint;
        _lastDragMousePosScreen      = AvaloniaMathExtension.ZeroPoint;
    }

    private void DragCenter(Point leftButtonPressedPos, Point mousePos, Matrix screenToWorld)
    {
        var dx = mousePos.X - leftButtonPressedPos.X + OriginPointerOffset.X;
        var dy = mousePos.Y - leftButtonPressedPos.Y + OriginPointerOffset.Y;

        var currentScreen = leftButtonPressedPos + new Vector(dx, dy);
        var currentWorld  = currentScreen * screenToWorld;

        Origin = new Point(currentWorld.X, currentWorld.Y);
    }

    private void DragTranslateX(Point leftButtonPressedPos, Point mousePos, Matrix screenToWorld, Matrix worldToScreen)
    {
        var pivotScreen = Origin * worldToScreen;
        var factor      = GetTranslateModifierFactor(mousePos.Y, pivotScreen.Y);

        var delta = mousePos - _lastDragMousePosScreen;
        _lastDragMousePosScreen = mousePos;

        _dragAccumulatedScreenOffset = new Point(
            _dragAccumulatedScreenOffset.X + delta.X * factor,
            _dragAccumulatedScreenOffset.Y);

        var currentScreen = leftButtonPressedPos + new Vector(_dragAccumulatedScreenOffset.X, 0) + OriginPointerOffset;
        var currentWorld  = screenToWorld.Transform(currentScreen);

        Origin = new Point(currentWorld.X, Origin.Y);
    }

    private void DragTranslateY(Point leftButtonPressedPos, Point mousePos, Matrix screenToWorld, Matrix worldToScreen)
    {
        var pivotScreen = Origin * worldToScreen;
        var factor      = GetTranslateModifierFactor(mousePos.X, pivotScreen.X);

        var delta = mousePos - _lastDragMousePosScreen;
        _lastDragMousePosScreen = mousePos;

        _dragAccumulatedScreenOffset = new Point(
            _dragAccumulatedScreenOffset.X,
            _dragAccumulatedScreenOffset.Y + delta.Y * factor);

        var currentScreen = leftButtonPressedPos + new Vector(0, _dragAccumulatedScreenOffset.Y) + OriginPointerOffset;
        var currentWorld  = currentScreen * screenToWorld;

        Origin = new Point(Origin.X, currentWorld.Y);
    }

    private void DragScaleX(Point mousePos, Matrix worldToScreen)
    {
        var pivot   = Origin * worldToScreen;
        var anchorX = pivot.X + InnerScaleOffset + InnerScaleLength;

        ScaleXVisualLen = mousePos.X - anchorX;
    }

    private void DragScaleY(Point mousePos, Matrix worldToScreen)
    {
        var pivot   = Origin * worldToScreen;
        var anchorY = pivot.Y + InnerScaleOffset + InnerScaleLength;

        ScaleYVisualLen = mousePos.Y - anchorY;
    }

    private void DragRotate(Point mousePos, Matrix worldToScreen)
    {
        var pivot = Origin * worldToScreen;

        var curVec = mousePos - pivot;
        var curMouseAngle = AvaloniaMathExtension.Atan2(curVec);

        var delta = AvaloniaMathExtension.WrapPi(curMouseAngle - RotateStartPointerAngle);
        Angle = RotateStartGizmoAngle + delta;
    }

    private Part HitTest(Point origin, Point mousePos)
    {
        if (GetCenterRect(origin).Contains(mousePos))
            return Part.Center;

        var scaleXEnd = GetScaleXEnd(origin);
        if (GetScaleHandleHitRect(scaleXEnd).Contains(mousePos))
            return Part.ScaleX;

        var scaleYEnd = GetScaleYEnd(origin);
        if (GetScaleHandleHitRect(scaleYEnd).Contains(mousePos))
            return Part.ScaleY;

        if (GetAxisBounds(GetScaleXStart(origin), scaleXEnd).Contains(mousePos))
            return Part.ScaleX;

        if (GetAxisBounds(GetScaleYStart(origin), scaleYEnd).Contains(mousePos))
            return Part.ScaleY;

        if (GetAxisBounds(GetTranslateXStart(origin), GetTranslateXEnd(origin)).Contains(mousePos))
            return Part.TranslateX;

        if (GetAxisBounds(GetTranslateYStart(origin), GetTranslateYEnd(origin)).Contains(mousePos))
            return Part.TranslateY;

        var dx = mousePos.X - origin.X;
        var dy = mousePos.Y - origin.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);

        if (Math.Abs(dist - RotateRadius) <= RotateHitBand)
            return Part.Rotate;

        return Part.None;
    }

    private static int ClampIndex(int i, int count)
    {
        if (i < 0)
            return 0;
        if (i >= count)
            return count - 1;
        return i;
    }

    private double GetTranslateModifierFactor(double axisPos, double pivotAxisPos)
    {
        var count = ModifierValues.Count;
        if (count == 0 || ModifierStep <= 0.0)
            return 1.0;

        var mid = (count - 1) * 0.5;

        var idx = (int)Math.Floor(((axisPos - pivotAxisPos) + (ModifierStep * 0.5)) / ModifierStep + mid);
        idx = ClampIndex(idx, count);

        return ModifierValues[idx];
    }

    private Rect GetCenterRect(Point origin)
    {
        var half = CenterHandleSize * 0.5;
        return new Rect(origin.X - half, origin.Y - half, CenterHandleSize, CenterHandleSize);
    }

    private Rect GetAxisBounds(Point a, Point b)
    {
        if (Math.Abs(a.Y - b.Y) < 1e-9)
        {
            var x0 = Math.Min(a.X, b.X);
            var w  = Math.Abs(b.X - a.X);
            return new Rect(x0, a.Y - AxisHitHalfThickness, w, AxisHitHalfThickness * 2);
        }

        var y0 = Math.Min(a.Y, b.Y);
        var h  = Math.Abs(b.Y - a.Y);
        return new Rect(a.X - AxisHitHalfThickness, y0, AxisHitHalfThickness * 2, h);
    }

    private Point GetScaleXStart(Point o) => new(o.X + InnerScaleOffset, o.Y);
    private Point GetScaleXEnd(Point o)   => new(o.X + InnerScaleOffset + InnerScaleLength, o.Y);
    private Point GetScaleYStart(Point o) => new(o.X, o.Y + InnerScaleOffset);
    private Point GetScaleYEnd(Point o)   => new(o.X, o.Y + InnerScaleOffset + InnerScaleLength);

    private Rect GetScaleHandleRect(Point end)
    {
        var half = ScaleHandleSize * 0.5;
        return new Rect(end.X - half, end.Y - half, ScaleHandleSize, ScaleHandleSize);
    }

    private Rect GetScaleHandleHitRect(Point end)
    {
        var r = GetScaleHandleRect(end);
        var inflate = Math.Max(3.0, AxisHitHalfThickness);
        return new Rect(r.X - inflate, r.Y - inflate, r.Width + 2 * inflate, r.Height + 2 * inflate);
    }

    private Point GetTranslateXStart(Point o)
    {
        var off = RotateRadius + OuterTranslateOffset;
        return new Point(o.X + off, o.Y);
    }

    private Point GetTranslateXEnd(Point o)
    {
        var off = RotateRadius + OuterTranslateOffset;
        return new Point(o.X + off + OuterTranslateLength, o.Y);
    }

    private Point GetTranslateYStart(Point o)
    {
        var off = RotateRadius + OuterTranslateOffset;
        return new Point(o.X, o.Y + off);
    }

    private Point GetTranslateYEnd(Point o)
    {
        var off = RotateRadius + OuterTranslateOffset;
        return new Point(o.X, o.Y + off + OuterTranslateLength);
    }

    private void OnTooltipTimerTick(object? sender, EventArgs e)
    {
        foreach (var button in _menuButtons)
            button.ShouldShowTooltip = button.IsHovered;

        RaiseRequestInvalidateVisual();
    }

    private void RaiseRequestInvalidateVisual() => RequestInvalidateVisual?.Invoke();

    public Part ActivePart  { get; private set; } = Part.None;
    public Part HoveredPart { get; private set; } = Part.None;

    public Point Origin { get; set; } = AvaloniaMathExtension.ZeroPoint;
    public Point OriginPointerOffset { get; set; } = AvaloniaMathExtension.ZeroPoint;

    public bool IsActive  => ActivePart != Part.None;
    public bool IsVisible { get; set; } = true;

    public double Angle { get; set; } = 0d;
    public double RotateStartGizmoAngle { get; set; }
    public double RotateStartPointerAngle { get; set; }

    public double ScaleXVisualLen { get; set; } = 0d;
    public double ScaleYVisualLen { get; set; } = 0d;

    public event Action? RequestInvalidateVisual;

    private Point _lastDragMousePosScreen = AvaloniaMathExtension.ZeroPoint;
    private Point _dragAccumulatedScreenOffset = AvaloniaMathExtension.ZeroPoint;

    private readonly DispatcherTimer _toolTipTimer = new()
    {
        Interval = TimeSpan.FromMilliseconds(250),
    };

    private IEnumerable<Gizmo2DMenuButton> _menuButtons = new List<Gizmo2DMenuButton>
    {
        new Gizmo2DMenuToggleButton(270, 0.0d, [
            new MultiStateToggleCreateInfo("menu_closed", "icon_menu", "Menu", 0.02, true),
            new MultiStateToggleCreateInfo("menu_closed", "icon_menu_open", "Menu", 0.02)
        ]),
        new Gizmo2DMenuToggleButton(330, 0.0d, [
            new MultiStateToggleCreateInfo("menu_global_coordinate_system", "icon_global_coordinate_system", "Global Space", 0.033, true),
            new MultiStateToggleCreateInfo("menu_local_coordinate_system", "icon_local_coordinate_system", "Local Space", 0.041)
        ]),
        
        new Gizmo2DMenuButton("icon_rotate_90_degrees_cw", "Rotate 90° Clockwise", 30, 0.0d, 0.02),
        new Gizmo2DMenuButton("icon_rotate_90_degrees_ccw", "Rotate 90° Counterclockwise", 60, 0.0d, 0.02),
    };
}
