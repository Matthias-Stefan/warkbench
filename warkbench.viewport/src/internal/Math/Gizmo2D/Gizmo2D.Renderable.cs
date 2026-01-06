using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using warkbench.core;


namespace warkbench.viewport;
internal partial class Gizmo2D
{
    /// <summary>Renders the gizmo using the current world-to-screen transform.</summary>
    public void Render(DrawingContext ctx, Matrix worldToScreenMatrix)
    {
        if (!IsVisible)
            return;

        var handleBrush = new SolidColorBrush(ViewportStyle.HandleColor);

        var handlePen        = new Pen(handleBrush, StrokeThickness);
        var handlePenFocused = new Pen(handleBrush, HoverThickness);

        _worldToScreenMatrix = worldToScreenMatrix;

        RenderTranslationHandle(ctx, handlePen, handlePenFocused);
        RenderRotationHandle(ctx, handlePen, handlePenFocused);
        RenderScaleHandle(ctx);
        RenderMenu(ctx);
    }

    /// <summary>Renders center handle and translation axes including modifier bands.</summary>
    private void RenderTranslationHandle(DrawingContext ctx, Pen handlePen, Pen handlePenFocused)
    {
        var origin = Origin * _worldToScreenMatrix;

        var centerRect      = GetCenterRect(origin);
        var centerIsFocused = IsFocused(Part.Center);

        ctx.DrawRectangle(
            new SolidColorBrush(Colors.Transparent),
            centerIsFocused ? handlePenFocused : handlePen,
            centerRect);

        var translateStartOffset = RotateRadius + OuterTranslateOffset;

        var translateXStart = new Point(origin.X + translateStartOffset, origin.Y);
        var translateXEnd   = new Point(origin.X + translateStartOffset + OuterTranslateLength, origin.Y);

        var translateYStart = new Point(origin.X, origin.Y + translateStartOffset);
        var translateYEnd   = new Point(origin.X, origin.Y + translateStartOffset + OuterTranslateLength);

        DrawTranslateAxis(ctx, translateXStart, translateXEnd, ViewportStyle.XColor, IsFocused(Part.TranslateX));
        DrawTranslateAxis(ctx, translateYStart, translateYEnd, ViewportStyle.YColor, IsFocused(Part.TranslateY));

        if (ModifierStep <= 0.0 || ModifierValues.Count == 0)
            return;

        var showX = IsHoveredOrActive(Part.TranslateX);
        var showY = IsHoveredOrActive(Part.TranslateY);

        if (showX)
        {
            var x0 = Math.Min(translateXStart.X, translateXEnd.X);
            var x1 = Math.Max(translateXStart.X, translateXEnd.X);
            var w  = Math.Max(1.0, x1 - x0);

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
            var y0 = Math.Min(translateYStart.Y, translateYEnd.Y);
            var y1 = Math.Max(translateYStart.Y, translateYEnd.Y);
            var h  = Math.Max(1.0, y1 - y0);

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
    private void RenderRotationHandle(DrawingContext ctx, Pen handlePen, Pen handlePenFocused)
    {
        var origin        = Origin * _worldToScreenMatrix;
        var ringIsFocused = IsFocused(Part.Rotate);

        ctx.DrawEllipse(
            null,
            ringIsFocused ? handlePenFocused : handlePen,
            origin,
            RotateRadius,
            RotateRadius);

        var dir            = new Vector(Math.Cos(Angle), Math.Sin(Angle));
        var ringInnerPoint = origin + dir * RotateRadius;
        var ringOuterPoint = origin + dir * (RotateRadius + IndicatorLength);

        ctx.DrawLine(
            ringIsFocused ? handlePenFocused : handlePen,
            ringInnerPoint,
            ringOuterPoint);
    }

    /// <summary>Renders scale axes with line and square handle.</summary>
    private void RenderScaleHandle(DrawingContext ctx)
    {
        var origin = Origin * _worldToScreenMatrix;

        var scaleXStart = new Point(origin.X + InnerScaleOffset, origin.Y);
        var scaleXEnd   = new Point(origin.X + InnerScaleOffset + InnerScaleLength + ScaleXVisualLen, origin.Y);

        var scaleYStart = new Point(origin.X, origin.Y + InnerScaleOffset);
        var scaleYEnd   = new Point(origin.X, origin.Y + InnerScaleOffset + InnerScaleLength + ScaleYVisualLen);

        DrawScaleAxis(ctx, scaleXStart, scaleXEnd, ViewportStyle.XColor, IsFocused(Part.ScaleX));
        DrawScaleAxis(ctx, scaleYStart, scaleYEnd, ViewportStyle.YColor, IsFocused(Part.ScaleY));
    }

    /// <summary></summary>
    private void RenderMenu(DrawingContext ctx)
    {
        var origin = Origin * _worldToScreenMatrix;

        if ( (!GetStreamGeometry("icon_menu", 270, 0.02, out var iconMenu) || iconMenu is null) ||
             (!GetStreamGeometry("icon_menu_open", 300, 0.02, out var iconMenuOpen) || iconMenuOpen is null) ||
             (!GetStreamGeometry("icon_rotate_90_degrees_cw", 180, 0.02, out var iconRotateCw) || iconRotateCw is null) ||
             (!GetStreamGeometry("icon_rotate_90_degrees_ccw", 210, 0.02, out var iconRotateCcw) || iconRotateCcw is null) ||
             (!GetStreamGeometry("icon_global_coordinate_system", 150, 0.03, out var iconGlobalCoordinateSystem) || iconGlobalCoordinateSystem is null) ||
             (!GetStreamGeometry("icon_local_coordinate_system", 120, 0.035, out var iconLocalCoordinateSystem) || iconLocalCoordinateSystem is null)
           )
            return;
        
        ctx.DrawGeometry(Brushes.White, null, iconMenu);
        ctx.DrawGeometry(Brushes.White, null, iconMenuOpen);
        ctx.DrawGeometry(Brushes.White, null, iconRotateCw);
        ctx.DrawGeometry(Brushes.White, null, iconRotateCcw);
        ctx.DrawGeometry(Brushes.White, null, iconGlobalCoordinateSystem);
        ctx.DrawGeometry(Brushes.White, null, iconLocalCoordinateSystem);
    }

    private bool GetStreamGeometry(string name, double degrees, double scale, [MaybeNullWhen(false)] out Geometry? geometry)
    {
        geometry = null;

        // Try to resolve the StreamGeometry from application resources
        if (Application.Current?.FindResource(name) is not StreamGeometry streamGeometry)
            return false;

        // Transform origin from world space to screen space
        var origin = Origin * _worldToScreenMatrix;
        var bounds = streamGeometry.Bounds;

        // Ring radius in screen space
        var r = MenuOffset + RotateRadius + IndicatorLength;

        // Angle in radians:
        // 0° = right, 90° = down, 180° = left, 270° = up
        var theta = degrees * Math.PI / 180.0;

        // Compute icon position on the ring
        var targetX = origin.X + Math.Cos(theta) * r;
        var targetY = origin.Y + Math.Sin(theta) * r;

        var g = streamGeometry.Clone();

        g.Transform = new TransformGroup
        {
            Children =
            {
                // 1) Move geometry center to (0,0) so scaling/rotation happens around its center
                new TranslateTransform(
                    -(bounds.X + bounds.Width  * 0.5),
                    -(bounds.Y + bounds.Height * 0.5)),

                // 2) Scale the icon to the desired size
                new ScaleTransform(scale, scale),

                // 3) Optional: rotate the icon itself
                //    - radial alignment:   RotateTransform(degrees)
                //    - tangential alignment: RotateTransform(degrees + 90)
                // new RotateTransform(degrees),

                // 4) Translate the icon to its final position on the ring
                new TranslateTransform(targetX, targetY),
            }
        };

        geometry = g;
        return true;
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

        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length < 1e-9)
            return WarkbenchMath.EmptyGeometry;

        var ux = dx / length;
        var uy = dy / length;

        var angleRad = headAngleDeg * Math.PI / 180.0;
        var sin      = Math.Sin(angleRad);
        var cos      = Math.Cos(angleRad);

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

            var normalized = Math.Clamp(Math.Log2(factor + 1.0) / 2.0, 0.0, 1.0);
            var intensity  = (byte)Math.Clamp(80 + normalized * 175, 0, 255);

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

            var normalized = Math.Clamp(Math.Log2(factor + 1.0) / 2.0, 0.0, 1.0);
            var intensity  = (byte)Math.Clamp(80 + normalized * 175, 0, 255);

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
