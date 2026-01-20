using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using warkbench.src.ui.core.Math;
using warkbench.src.ui.core.Themes;


namespace warkbench.src.ui.core.Gizmo;

public partial class Gizmo2D
{
    /// <summary>Renders the gizmo using the current world-to-screen transform.</summary>
    public void Render(DrawingContext ctx, Matrix worldToScreenMatrix, Point mousePos)
    {
        if (!IsVisible)
            return;

        var handleBrush = new SolidColorBrush(ViewportStyle.HandleColor);

        var handlePen = new Pen(handleBrush, StrokeThickness);
        var handlePenFocused = new Pen(handleBrush, HoverThickness);

        RenderTranslationHandle(ctx, worldToScreenMatrix, handlePen, handlePenFocused);
        RenderRotationHandle(ctx, worldToScreenMatrix, handlePen, handlePenFocused);
        RenderScaleHandle(ctx, worldToScreenMatrix);
        RenderMenu(ctx, worldToScreenMatrix, mousePos);
    }

    /// <summary>Renders center handle and translation axes including modifier bands.</summary>
    private void RenderTranslationHandle(DrawingContext ctx, Matrix worldToScreenMatrix, Pen handlePen, Pen handlePenFocused)
    {
        var origin = Origin * worldToScreenMatrix;

        var centerRect = GetCenterRect(origin);
        var centerIsFocused = IsFocused(Part.Center);

        ctx.DrawRectangle(
            new SolidColorBrush(Colors.Transparent),
            centerIsFocused ? handlePenFocused : handlePen,
            centerRect);

        var translateStartOffset = RotateRadius + OuterTranslateOffset;

        var translateXStart = new Point(origin.X + translateStartOffset, origin.Y);
        var translateXEnd = new Point(origin.X + translateStartOffset + OuterTranslateLength, origin.Y);

        var translateYStart = new Point(origin.X, origin.Y + translateStartOffset);
        var translateYEnd = new Point(origin.X, origin.Y + translateStartOffset + OuterTranslateLength);

        DrawTranslateAxis(ctx, translateXStart, translateXEnd, ViewportStyle.XColor, IsFocused(Part.TranslateX));
        DrawTranslateAxis(ctx, translateYStart, translateYEnd, ViewportStyle.YColor, IsFocused(Part.TranslateY));

        if (ModifierStep <= 0.0 || ModifierValues.Count == 0)
            return;

        var showX = IsHoveredOrActive(Part.TranslateX);
        var showY = IsHoveredOrActive(Part.TranslateY);

        if (showX)
        {
            var x0 = System.Math.Min(translateXStart.X, translateXEnd.X);
            var x1 = System.Math.Max(translateXStart.X, translateXEnd.X);
            var w = System.Math.Max(1.0, x1 - x0);

            DrawModifierBandsVertical(
                ctx,
                x0 + ModifierOffset,
                translateXStart.Y,
                w,
                ModifierStep,
                ModifierValues,
                baseColor: new Color(255, 255, 60, 60));
        }
        else if (showY)
        {
            var y0 = System.Math.Min(translateYStart.Y, translateYEnd.Y);
            var y1 = System.Math.Max(translateYStart.Y, translateYEnd.Y);
            var h = System.Math.Max(1.0, y1 - y0);

            DrawModifierBandsHorizontal(
                ctx,
                translateYStart.X,
                y0 + ModifierOffset,
                ModifierStep,
                h,
                ModifierValues,
                baseColor: new Color(255, 60, 255, 60));
        }
    }

    /// <summary>Renders the rotation ring and facing-direction indicator.</summary>
    private void RenderRotationHandle(DrawingContext ctx, Matrix worldToScreenMatrix, Pen handlePen, Pen handlePenFocused)
    {
        var origin = Origin * worldToScreenMatrix;
        var ringIsFocused = IsFocused(Part.Rotate);

        ctx.DrawEllipse(
            null,
            ringIsFocused ? handlePenFocused : handlePen,
            origin,
            RotateRadius,
            RotateRadius);

        var dir = new Vector(System.Math.Cos(Angle), System.Math.Sin(Angle));
        var ringInnerPoint = origin + dir * RotateRadius;
        var ringOuterPoint = origin + dir * (RotateRadius + IndicatorLength);

        ctx.DrawLine(
            ringIsFocused ? handlePenFocused : handlePen,
            ringInnerPoint,
            ringOuterPoint);
    }

    /// <summary>Renders scale axes with line and square handle.</summary>
    private void RenderScaleHandle(DrawingContext ctx, Matrix worldToScreenMatrix)
    {
        var origin = Origin * worldToScreenMatrix;

        var scaleXStart = new Point(origin.X + InnerScaleOffset, origin.Y);
        var scaleXEnd = new Point(origin.X + InnerScaleOffset + InnerScaleLength + ScaleXVisualLen, origin.Y);

        var scaleYStart = new Point(origin.X, origin.Y + InnerScaleOffset);
        var scaleYEnd = new Point(origin.X, origin.Y + InnerScaleOffset + InnerScaleLength + ScaleYVisualLen);

        DrawScaleAxis(ctx, scaleXStart, scaleXEnd, ViewportStyle.XColor, IsFocused(Part.ScaleX));
        DrawScaleAxis(ctx, scaleYStart, scaleYEnd, ViewportStyle.YColor, IsFocused(Part.ScaleY));
    }
    
    /// <summary></summary>
    private void RenderMenu(DrawingContext ctx, Matrix worldToScreenMatrix, Point mousePos)
    {
        // Transform origin from world space to screen space
        var origin = Origin * worldToScreenMatrix;

        // Ring radius in screen space
        var ringRadiusScreenSpace = MenuOffset + RotateRadius + IndicatorLength;
        
        foreach (var menuButton in _menuButtons)
        {
            menuButton.Render(ctx, origin, mousePos, ringRadiusScreenSpace);
        }
    }

    private bool IsFocused(Part part)
        => ActivePart == part || (HoveredPart == part && !IsActive);

    private bool IsHoveredOrActive(Part part)
        => (HoveredPart == part && ActivePart == Part.None) || ActivePart == part;

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
        var brush     = new SolidColorBrush(color);
        var thickness = focused ? HoverThickness : StrokeThickness;

        ctx.DrawLine(new Pen(brush, thickness), start, end);

        var half       = ScaleHandleSize * 0.5;
        var handleRect = new Rect(end.X - half, end.Y - half, ScaleHandleSize, ScaleHandleSize);

        ctx.DrawGeometry(
            brush,
            new Pen(brush, thickness),
            new RectangleGeometry(handleRect));
    }

    /// <summary>
    /// Constructs a stroke-only arrow <see cref="Geometry"/> by deriving a normalized direction vector
    /// from <paramref name="start"/> to <paramref name="end"/> and generating two symmetric head segments
    /// via 2D vector rotation using <paramref name="headAngleDeg"/>. Returns an empty geometry for
    /// degenerate (near-zero) shaft length.
    /// </summary>
    private static Geometry CreateTranslateGeometry(Point start, Point end, double headLength, double headAngleDeg)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        var length = System.Math.Sqrt(dx * dx + dy * dy);
        if (length < 1e-9)
            return AvaloniaMathExtension.EmptyGeometry;

        var ux = dx / length;
        var uy = dy / length;

        var angleRad = headAngleDeg * System.Math.PI / 180.0;
        var sin      = System.Math.Sin(angleRad);
        var cos      = System.Math.Cos(angleRad);

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

    /// <summary>
    /// Renders a vertical modifier overlay as a set of equally sized bands centered around <paramref name="pivotY"/>.
    /// Each band represents one discrete scale factor and is drawn as a translucent rectangle with a centered label.
    /// </summary>
    private static void DrawModifierBandsVertical(
        DrawingContext ctx,
        double xLeft,
        double pivotY,
        double width,
        double step,
        IReadOnlyList<double> factors,
        Color baseColor)
    {
        var count = factors.Count;
        if (count == 0)
            return;

        var mid = (count - 1) * 0.5;

        for (var i = 0; i < count; i++)
        {
            var factor = factors[i];

            var offset = (i - mid) * step;
            var yTop   = pivotY + offset - (step * 0.5);

            var normalized = System.Math.Clamp(System.Math.Log2(factor + 1.0) / 2.0, 0.0, 1.0);
            var intensity  = (byte)System.Math.Clamp(80 + normalized * 175, 0, 255);

            var color = Color.FromArgb(128, intensity, baseColor.G, baseColor.B);

            ctx.DrawRectangle(
                new SolidColorBrush(color),
                null,
                new Rect(new Point(xLeft, yTop), new Size(width, step)));

            var text = $"{factor:0.##}x";
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                10,
                new SolidColorBrush(Colors.White));

            ctx.DrawText(
                ft,
                new Point(
                    xLeft + (width - ft.Width) * 0.5,
                    yTop  + (step  - ft.Height) * 0.5));
        }
    }

    /// <summary>
    /// Renders a horizontal modifier overlay as a set of equally sized bands centered around <paramref name="pivotX"/>.
    /// Each band represents one discrete scale factor and is drawn as a translucent rectangle with a centered label.
    /// </summary>
    private static void DrawModifierBandsHorizontal(
        DrawingContext ctx,
        double pivotX,
        double yTop,
        double step,
        double height,
        IReadOnlyList<double> factors,
        Color baseColor)
    {
        var count = factors.Count;
        if (count == 0)
            return;

        var mid = (count - 1) * 0.5;

        for (var i = 0; i < count; i++)
        {
            var factor = factors[i];

            var offset = (i - mid) * step;
            var xLeft  = pivotX + offset - (step * 0.5);

            var normalized = System.Math.Clamp(System.Math.Log2(factor + 1.0) / 2.0, 0.0, 1.0);
            var intensity  = (byte)System.Math.Clamp(80 + normalized * 175, 0, 255);

            var color = Color.FromArgb(128, baseColor.R, intensity, baseColor.B);

            ctx.DrawRectangle(
                new SolidColorBrush(color),
                null,
                new Rect(new Point(xLeft, yTop), new Size(step, height)));

            var text = $"{factor:0.##}x";
            var ft = new FormattedText(
                text,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                10,
                new SolidColorBrush(Colors.White));

            ctx.DrawText(
                ft,
                new Point(
                    xLeft + (step - ft.Width) * 0.5,
                    yTop  + (height - ft.Height) * 0.5));
        }
    }
}
