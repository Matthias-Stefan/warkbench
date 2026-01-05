using System;
using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
internal partial class Gizmo2D
{
    /// <summary>Renders the gizmo using the current world-to-screen transform.</summary>
    public void Render(DrawingContext ctx, Matrix worldToScreenMatrix)
    {
        if (!IsVisible)
            return;

        var handleThickness= StrokeThickness;
        var focusedThickness= HoverThickness;

        var handleBrush = new SolidColorBrush(ViewportStyle.HandleColor);
        var handlePen = new Pen(handleBrush, handleThickness);
        var handlePenFocused = new Pen(handleBrush, focusedThickness);

        _worldToScreenMatrix = worldToScreenMatrix;

        RenderTranslationHandle(ctx, handlePen, handlePenFocused);
        RenderRotationHandle(ctx, handlePen, handlePenFocused);
        RenderScaleHandle(ctx, handleThickness, focusedThickness);
    }

    /// <summary>Renders center handle and translation axes including modifier bands.</summary>
    private void RenderTranslationHandle(DrawingContext ctx, Pen handlePen, Pen handlePenFocused)
    {
        var origin= Origin * _worldToScreenMatrix;

        var centerRect = GetCenterRect(origin);
        var centerIsFocused= ActivePart == Part.Center || (HoveredPart == Part.Center && !IsActive);
        ctx.DrawRectangle(
            new SolidColorBrush(Colors.Transparent),
            centerIsFocused ? handlePenFocused : handlePen,
            centerRect);

        var translateStartOffset = RotateRadius + OuterTranslateOffset;

        var translateXStart = new Point(origin.X + translateStartOffset, origin.Y);
        var translateXEnd = new Point(origin.X + translateStartOffset + OuterTranslateLength, origin.Y);

        var translateYStart = new Point(origin.X, origin.Y + translateStartOffset);
        var translateYEnd = new Point(origin.X, origin.Y + translateStartOffset + OuterTranslateLength);

        var xHandleIsFocused= ActivePart == Part.TranslateX || (HoveredPart == Part.TranslateX && !IsActive);
        var yHandleIsFocused= ActivePart == Part.TranslateY || (HoveredPart == Part.TranslateY && !IsActive);

        DrawTranslateAxis(ctx, translateXStart, translateXEnd, ViewportStyle.XColor, xHandleIsFocused);
        DrawTranslateAxis(ctx, translateYStart, translateYEnd, ViewportStyle.YColor, yHandleIsFocused);

        if (ModifierCount < 2 || ModifierStep <= 0.0)
            return;

        var showX= (HoveredPart == Part.TranslateX && ActivePart == Part.None) || ActivePart == Part.TranslateX;
        var showY= (HoveredPart == Part.TranslateY && ActivePart == Part.None) || ActivePart == Part.TranslateY;

        if (showX)
        {
            var x0= Math.Min(translateXStart.X, translateXEnd.X);
            var x1= Math.Max(translateXStart.X, translateXEnd.X);
            var w= Math.Max(1.0, x1 - x0);

            DrawModifierBandsVertical(
                ctx,
                x0,
                translateXStart.Y,
                w,
                ModifierStep,
                ModifierCount,
                baseColor: new Color(255, 255, 60, 60));
        }
        else if (showY)
        {
            var y0= Math.Min(translateYStart.Y, translateYEnd.Y);
            var y1= Math.Max(translateYStart.Y, translateYEnd.Y);
            var h= Math.Max(1.0, y1 - y0);

            DrawModifierBandsHorizontal(
                ctx,
                translateYStart.X,
                y0,
                ModifierStep,
                h,
                ModifierCount,
                baseColor: new Color(255, 60, 255, 60));
        }
    }

    /// <summary>Renders the rotation ring and facing-direction indicator.</summary>
    private void RenderRotationHandle(DrawingContext ctx, Pen handlePen, Pen handlePenFocused)
    {
        var origin= Origin * _worldToScreenMatrix;
        var ringIsFocused= ActivePart == Part.Rotate || (HoveredPart == Part.Rotate && !IsActive);

        ctx.DrawEllipse(
            null,
            ringIsFocused ? handlePenFocused : handlePen,
            origin,
            RotateRadius,
            RotateRadius);

        var dir = new Vector(Math.Cos(Angle), Math.Sin(Angle));
        var ringInnerPoint = origin + dir * RotateRadius;
        var ringOuterPoint = origin + dir * (RotateRadius + IndicatorLength);

        ctx.DrawLine(
            ringIsFocused ? handlePenFocused : handlePen,
            ringInnerPoint,
            ringOuterPoint);
    }

    private Point Blah = WarkbenchMath.ZeroPoint;
    
    /// <summary>Renders scale axes with line and square handle.</summary>
    private void RenderScaleHandle(DrawingContext ctx, double thickness, double focusedThickness)
    {
        var origin = Origin * _worldToScreenMatrix;

        var scaleXStart = new Point(origin.X + InnerScaleOffset, origin.Y);
        var scaleXEnd = new Point(origin.X + InnerScaleOffset + InnerScaleLength + Blah.X, origin.Y);

        var scaleYStart = new Point(origin.X, origin.Y + InnerScaleOffset);
        var scaleYEnd = new Point(origin.X, origin.Y + InnerScaleOffset + InnerScaleLength);
        
        
        
        var xHandleIsFocused = ActivePart == Part.ScaleX || (HoveredPart == Part.ScaleX && !IsActive);
        var yHandleIsFocused = ActivePart == Part.ScaleY || (HoveredPart == Part.ScaleY && !IsActive);
        
        DrawScaleAxis(ctx, scaleXStart, scaleXEnd, ViewportStyle.XColor, xHandleIsFocused);
        DrawScaleAxis(ctx, scaleYStart, scaleYEnd, ViewportStyle.YColor, yHandleIsFocused);
    }
    
    /// <summary>Draws a translation axis as an arrow (stroke only).</summary>
    private void DrawTranslateAxis(DrawingContext ctx, Point start, Point end, Color color, bool focused)
    {
        var pen = new Pen(new SolidColorBrush(color), focused ? HoverThickness : StrokeThickness);

        var geo = CreateTranslateGeometry(start, end, headLength: 6, headAngleDeg: 30);
        ctx.DrawGeometry(null, pen, geo);
    }

    /// <summary>Draws a scale axis as a line with a square handle at the end.</summary>
    private void DrawScaleAxis(DrawingContext ctx, Point start, Point end, Color color, bool focused)
    {
        var brush = new SolidColorBrush(color);
        var thickness = focused ? HoverThickness : StrokeThickness;

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
    
    /// <summary>Creates arrow geometry consisting of a line and arrow head.</summary>
    private static Geometry CreateTranslateGeometry(Point start, Point end, double headLength, double headAngleDeg)
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
    
    /// <summary>Draws vertical modifier bands along the X axis.</summary>
    private static void DrawModifierBandsVertical(
        DrawingContext ctx,
        double xLeft,
        double pivotY,
        double width,
        double step,
        int count,
        Color baseColor)
    {
        var mid = (count - 1) * 0.5;

        for (var i = 0; i < count; i++)
        {
            var offset = (i - mid) * step;
            var yTop = pivotY + offset - (step * 0.5);

            var t = 1.0 - (i / (count - 1.0));
            var intensity = (byte)Math.Clamp(80 + t * 175, 0, 255);

            var c = Color.FromArgb(128, intensity, baseColor.G, baseColor.B);
            ctx.DrawRectangle(
                new SolidColorBrush(c),
                null,
                new Rect(new Point(xLeft, yTop), new Size(width, step))
            );
        }
    }

    /// <summary>Draws horizontal modifier bands along the Y axis.</summary>
    private static void DrawModifierBandsHorizontal(
        DrawingContext ctx,
        double pivotX,
        double yTop,
        double step,
        double height,
        int count,
        Color baseColor)
    {
        var mid = (count - 1) * 0.5;

        for (var i = 0; i < count; i++)
        {
            var offset = (i - mid) * step;
            var xLeft = pivotX + offset - (step * 0.5);

            var t = 1.0 - (i / (count - 1.0));
            var intensity = (byte)Math.Clamp(80 + t * 175, 0, 255);

            var c = Color.FromArgb(128, baseColor.R, intensity, baseColor.B);
            ctx.DrawRectangle(
                new SolidColorBrush(c),
                null,
                new Rect(new Point(xLeft, yTop), new Size(step, height))
            );
        }
    }
}
