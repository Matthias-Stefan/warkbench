using System;
using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
internal class GridRenderer
{
    public void Render(DrawingContext ctx, ViewportCamera cam, Rect viewportBounds)
    {
        using var _ = ctx.PushClip(viewportBounds);

        var size = viewportBounds.Size;

        const int majorEvery = 8;
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

        for (var x = startX; x <= endX; x += spacing)
        {
            var xi = (int)Math.Round(x / spacing);
            var pen = (xi % majorEvery == 0) ? majorPen : minorPen;

            var a = cam.WorldToScreen(new Point(x, minY), size);
            var b = cam.WorldToScreen(new Point(x, maxY), size);

            a = new Point(Snap1Px(a.X), a.Y);
            b = new Point(Snap1Px(b.X), b.Y);

            ctx.DrawLine(pen, a, b);
        }

        for (var y = startY; y <= endY; y += spacing)
        {
            var yi = (int)Math.Round(y / spacing);
            var pen = (yi % majorEvery == 0) ? majorPen : minorPen;

            var a = cam.WorldToScreen(new Point(minX, y), size);
            var b = cam.WorldToScreen(new Point(maxX, y), size);

            a = new Point(a.X, Snap1Px(a.Y));
            b = new Point(b.X, Snap1Px(b.Y));

            ctx.DrawLine(pen, a, b);
        }
    }
    
    public void RenderOrigin(DrawingContext ctx, ViewportCamera cam, Rect viewportBounds)
    {
        var size = viewportBounds.Size;
        var origin = cam.WorldToScreen(new Point(0, 0), size);

        const double axisLength = 20;

        var xPen = new Pen(new SolidColorBrush(Color.FromArgb(220, 220, 60, 60)), 2);
        var yPen = new Pen(new SolidColorBrush(Color.FromArgb(220, 60, 220, 60)), 2);

        ctx.DrawLine(xPen, origin, origin + new Vector(axisLength, 0));
        ctx.DrawLine(yPen, origin, origin + new Vector(0, -axisLength));
    }
    
    private static double Snap1Px(double v) => Math.Floor(v) + 0.5;
    
    public double GridSpacing { get; set; } = 64.0;
}