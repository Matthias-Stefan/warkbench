using System;
using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
internal class Gizmo2D
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

    public void Render(DrawingContext ctx, Point origin)
    {
        if (!IsVisible)
        {
            return;
        }

        // Center handle
        var centerRect = GetCenterRect(origin);
        var centerIsHot = HoveredPart == Part.Center;

        ctx.DrawRectangle(
            new SolidColorBrush(Colors.Transparent),
            new Pen(new SolidColorBrush(RingColor), centerIsHot ? HoverThickness : StrokeThickness),
            centerRect);

        // Rotation ring
        var ringIsHot = HoveredPart == Part.Rotate;
        ctx.DrawEllipse(
            null,
            new Pen(new SolidColorBrush(RingColor), ringIsHot ? HoverThickness : StrokeThickness),
            origin,
            RotateRadius,
            RotateRadius);

        // Scale
        var scaleXStart = new Point(origin.X + InnerScaleOffset, origin.Y);
        var scaleXEnd   = new Point(origin.X + InnerScaleOffset + InnerScaleLength, origin.Y);

        var scaleYStart = new Point(origin.X, origin.Y + InnerScaleOffset);
        var scaleYEnd   = new Point(origin.X, origin.Y + InnerScaleOffset + InnerScaleLength);

        DrawScaleAxis(ctx, scaleXStart, scaleXEnd, XColor, HoveredPart == Part.ScaleX);
        DrawScaleAxis(ctx, scaleYStart, scaleYEnd, YColor, HoveredPart == Part.ScaleY);

        // Translate
        var translateStartOffset = RotateRadius + OuterTranslateOffset;
        var translateXStart = new Point(origin.X + translateStartOffset, origin.Y);
        var translateXEnd   = new Point(origin.X + translateStartOffset + OuterTranslateLength, origin.Y);

        var translateYStart = new Point(origin.X, origin.Y + translateStartOffset);
        var translateYEnd   = new Point(origin.X, origin.Y + translateStartOffset + OuterTranslateLength);

        DrawTranslateAxis(ctx, translateXStart, translateXEnd, XColor, HoveredPart == Part.TranslateX);
        DrawTranslateAxis(ctx, translateYStart, translateYEnd, YColor, HoveredPart == Part.TranslateY);
    }

    public bool OnPointerPressed(Point origin, Point mousePos)
    {
        var hit = HitTest(origin, mousePos);
        if (hit == Part.None)
        {
            return false;    
        }
        
        ActivePart = hit;
        return true;
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
    
    private void DrawTranslateAxis(DrawingContext ctx, Point start, Point end, Color color, bool hot)
    {
        var pen = new Pen(new SolidColorBrush(color), hot ? HoverThickness : StrokeThickness);

        var geo = CreateTranslateGeometry(start, end, headLength: 6, headAngleDeg: 30);
        ctx.DrawGeometry(null, pen, geo);
    }

    private void DrawScaleAxis(DrawingContext ctx, Point start, Point end, Color color, bool hot)
    {
        var brush = new SolidColorBrush(color);
        var thickness = hot ? HoverThickness : StrokeThickness;

        // axis line
        ctx.DrawLine(new Pen(brush, thickness), start, end);

        // filled square handle at end
        var half = ScaleHandleSize * 0.5;
        var handleRect = new Rect(end.X - half, end.Y - half, ScaleHandleSize, ScaleHandleSize);
        ctx.DrawGeometry(
            brush,
            new Pen(brush, thickness),
            new RectangleGeometry(handleRect));
    }
    
    private Geometry CreateTranslateGeometry(Point start, Point end, double headLength, double headAngleDeg)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 1e-9)
        {
            return WarkbenchMath.EmptyGeometry;    
        }

        var ux = dx / length;
        var uy = dy / length;

        var angleRad = headAngleDeg * Math.PI / 180.0;
        var sin = Math.Sin(angleRad);
        var cos = Math.Cos(angleRad);

        // rotate dir by +/- angle
        var lx =  cos * ux - sin * uy;
        var ly =  sin * ux + cos * uy;

        var rx =  cos * ux + sin * uy;
        var ry = -sin * ux + cos * uy;

        var left  = new Point(end.X - headLength * lx, end.Y - headLength * ly);
        var right = new Point(end.X - headLength * rx, end.Y - headLength * ry);

        var geo = new StreamGeometry();
        using var gctx = geo.Open();
        gctx.BeginFigure(start, false);
        gctx.LineTo(end);

        gctx.BeginFigure(left, false);
        gctx.LineTo(end);
        gctx.LineTo(right);

        return geo;
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

    public bool IsVisible { get; set; } = true;
    public Part ActivePart { get; private set; } = Part.None;
    
    public double StrokeThickness { get; set; } = 2d;
    public double HoverThickness  { get; set; } = 4d;
    public double CenterHandleSize { get; set; } = 8d;
    public double RotateRadius { get; set; } = 52d;
    public double RotateHitBand { get; set; } = 8d;
    public double InnerScaleOffset { get; set; } = 14d;
    public double InnerScaleLength { get; set; } = 18d;
    public double ScaleHandleSize  { get; set; } = 8d;
    public double OuterTranslateOffset { get; set; } = 18d;
    public double OuterTranslateLength { get; set; } = 30d;
    public double AxisHitHalfThickness { get; set; } = 6d;
    
    public Color XColor { get; set; } = Colors.IndianRed;
    public Color YColor { get; set; } = Colors.SeaGreen;
    public Color RingColor { get; set; } = Colors.DodgerBlue;
    
    public Part HoveredPart { get; private set; } = Part.None;
}