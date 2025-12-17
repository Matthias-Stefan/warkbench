using System;
using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
public class GridRenderer
{
    public void Render(DrawingContext ctx, Rect viewportBounds, ViewportCamera cam)
    {
        var size = viewportBounds.Size;

        var majorEvery = 8;
        var minorPen = new Pen(new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)), 1);
        var majorPen = new Pen(new SolidColorBrush(Color.FromArgb(60, 255, 255, 255)), 1);

        var worldTopLeft = cam.ScreenToWorld(new Point(0, 0), size);
        var worldBottomRight = cam.ScreenToWorld(new Point(size.Width, size.Height), size);

        var minX = Math.Min(worldTopLeft.X, worldBottomRight.X);
        var maxX = Math.Max(worldTopLeft.X, worldBottomRight.X);
        var minY = Math.Min(worldTopLeft.Y, worldBottomRight.Y);
        var maxY = Math.Max(worldTopLeft.Y, worldBottomRight.Y);

        var spacing = Math.Max(2.0, GridSpacing);

        var startX = Math.Floor(minX / spacing) * spacing;
        var endX = Math.Ceiling(maxX / spacing) * spacing;

        var startY = Math.Floor(minY / spacing) * spacing;
        var endY = Math.Ceiling(maxY / spacing) * spacing;

        var xIndex = 0;
        for (var x = startX; x <= endX; x += spacing, xIndex++)
        {
            var pen = (xIndex % majorEvery == 0) ? majorPen : minorPen;

            var a = cam.WorldToScreen(new Point(x, minY), size);
            var b = cam.WorldToScreen(new Point(x, maxY), size);

            ctx.DrawLine(pen, a, b);
        }

        var yIndex = 0;
        for (var y = startY; y <= endY; y += spacing, yIndex++)
        {
            var pen = (yIndex % majorEvery == 0) ? majorPen : minorPen;

            var a = cam.WorldToScreen(new Point(minX, y), size);
            var b = cam.WorldToScreen(new Point(maxX, y), size);

            ctx.DrawLine(pen, a, b);
        }
    }
    
    public void RenderOriginGizmo(DrawingContext ctx, Rect viewportBounds, ViewportCamera cam)
    {
        var size = viewportBounds.Size;
        var origin = cam.WorldToScreen(new Point(0, 0), size);

        const double axisLength = 20;

        var xPen = new Pen(new SolidColorBrush(Color.FromArgb(220, 220, 60, 60)), 2);
        var yPen = new Pen(new SolidColorBrush(Color.FromArgb(220, 60, 220, 60)), 2);

        ctx.DrawLine(xPen, origin, origin + new Vector(axisLength, 0));
        ctx.DrawLine(yPen, origin, origin + new Vector(0, -axisLength));
    }
    
    public double GridSpacing { get; set; } = 64.0;
}