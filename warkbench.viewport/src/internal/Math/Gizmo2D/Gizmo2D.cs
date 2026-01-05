using System;
using Avalonia;


namespace warkbench.viewport;
internal partial class Gizmo2D
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
    
    public void UpdateHover(Point origin, Point mousePos)
    {
        HoveredPart = HitTest(origin, mousePos);
    }

    public bool OnPointerPressed(Point origin, Point mousePos)
    {
        var hit = HitTest(origin, mousePos);
        if (hit == Part.None)
        {
            return false;    
        }
        
        ActivePart = hit;
        OriginPointerOffset = origin - mousePos;
        
        // Rotation
        if (hit == Part.Rotate)
        {
            var startVec = mousePos - origin;
            RotateStartPointerAngle = WarkbenchMath.Atan2(startVec);
            RotateStartGizmoAngle = Angle;
        }

        return true;
    }

    public void OnPointerMoved(
        Avalonia.Point leftButtonPressedPos,
        Avalonia.Point mousePos,
        Matrix screenToWorld,
        Matrix worldToScreen)
    {
        if (!IsActive)
        {
            return;
        }
        
        var dx = mousePos.X - leftButtonPressedPos.X + OriginPointerOffset.X;
        var dy = mousePos.Y - leftButtonPressedPos.Y + OriginPointerOffset.Y;
            
        switch (ActivePart)
        {
            // Translate
            case Gizmo2D.Part.Center:
                var currentScreen = leftButtonPressedPos + new Vector(dx, dy);
                var currentWorld = currentScreen * screenToWorld;
                Origin = new Point(currentWorld.X, currentWorld.Y);
                break;
            case Gizmo2D.Part.TranslateX:
                currentScreen = leftButtonPressedPos + new Vector(dx, 0);
                currentWorld = screenToWorld.Transform(currentScreen);
                Origin = new Point(currentWorld.X, Origin.Y);
                break;
            case Gizmo2D.Part.TranslateY:
                currentScreen = leftButtonPressedPos + new Vector(0, dy);
                currentWorld = currentScreen * screenToWorld;
                Origin = new Point(Origin.X, currentWorld.Y);
                break;
            
            // Scale
            case Gizmo2D.Part.ScaleX:
                var pivot = Origin * worldToScreen;
                Blah = mousePos - pivot;
                break;
            case Gizmo2D.Part.ScaleY:
                break;
            
            // Rotate
            case Gizmo2D.Part.Rotate:
                pivot = Origin * worldToScreen;

                var curVec = mousePos - pivot; // Screen - Screen
                var curMouseAngle = WarkbenchMath.Atan2(curVec);

                var delta = WarkbenchMath.WrapPi(curMouseAngle - RotateStartPointerAngle);

                Angle = RotateStartGizmoAngle + delta;
                break;
        }
    }

    public void OnPointerReleased()
    {
        ActivePart = Part.None;
    }
    
    private Part HitTest(Point origin, Point mousePos)
    {
        // center
        if (GetCenterRect(origin).Contains(mousePos))
        {
            return Part.Center;    
        }
        
        // scale inner (prefer scale over ring when inside)
        if (GetAxisBounds(GetScaleXStart(origin), GetScaleXEnd(origin)).Contains(mousePos))
        {
            return Part.ScaleX;    
        }
        if (GetAxisBounds(GetScaleYStart(origin), GetScaleYEnd(origin)).Contains(mousePos))
        {
            return Part.ScaleY;    
        }
        
        // translate outer
        if (GetAxisBounds(GetTranslateXStart(origin), GetTranslateXEnd(origin)).Contains(mousePos))
        {
            return Part.TranslateX;    
        }
        if (GetAxisBounds(GetTranslateYStart(origin), GetTranslateYEnd(origin)).Contains(mousePos))
        {
            return Part.TranslateY;    
        }

        // ring (distance band)
        var dx = mousePos.X - origin.X;
        var dy = mousePos.Y - origin.Y;
        var dist = Math.Sqrt(dx * dx + dy * dy);
        if (Math.Abs(dist - RotateRadius) <= RotateHitBand)
        {
            return Part.Rotate;    
        }

        return Part.None;
    }
    
    private Rect GetCenterRect(Point origin)
    {
        var half = CenterHandleSize * 0.5;
        return new Rect(origin.X - half, origin.Y - half, CenterHandleSize, CenterHandleSize);
    }

    private Rect GetAxisBounds(Point a, Point b)
    {
        // axis-aligned fat rect for easy picking
        if (Math.Abs(a.Y - b.Y) < 1e-9)
        {
            var x0 = Math.Min(a.X, b.X);
            var w  = Math.Abs(b.X - a.X);
            return new Rect(x0, a.Y - AxisHitHalfThickness, w, AxisHitHalfThickness * 2);
        }
        else
        {
            var y0 = Math.Min(a.Y, b.Y);
            var h  = Math.Abs(b.Y - a.Y);
            return new Rect(a.X - AxisHitHalfThickness, y0, AxisHitHalfThickness * 2, h);
        }
    }

    private Point GetScaleXStart(Point o) => new(o.X + InnerScaleOffset, o.Y);
    private Point GetScaleXEnd(Point o)   => new(o.X + InnerScaleOffset + InnerScaleLength, o.Y);
    private Point GetScaleYStart(Point o) => new(o.X, o.Y + InnerScaleOffset);
    private Point GetScaleYEnd(Point o)   => new(o.X, o.Y + InnerScaleOffset + InnerScaleLength);

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

    public Part ActivePart { get; private set; } = Part.None;
    public Part HoveredPart { get; private set; } = Part.None;
    public Point Origin { get; set; } = WarkbenchMath.ZeroPoint;
    public Point OriginPointerOffset { get; set; } = WarkbenchMath.ZeroPoint;
    
    public bool IsActive => ActivePart != Part.None;
    public bool IsVisible { get; set; } = true;
    
    public double Angle { get; set; } = 0d;
    public double RotateStartGizmoAngle { get; set; }
    public double RotateStartPointerAngle { get; set; }

    
    private Matrix _worldToScreenMatrix;
}