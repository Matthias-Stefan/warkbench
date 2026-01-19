using System;
using Avalonia;
using Avalonia.Media;

namespace warkbench.Brushes;

public static class PropertyBackgroundBrush
{
    public static IBrush Get()
    {
        var background = new GeometryDrawing
        {
            Geometry = new RectangleGeometry(new Rect(0, 0, 40, 40)),
            Brush = new SolidColorBrush(Color.Parse("#520000"))
        };

        var hexGroup = new GeometryGroup();

        var radius = 7.5;
        var vertOffset = radius * Math.Sqrt(3) / 2.0;

        AddHexagon(hexGroup, new Point(10, 10), radius);
        AddHexagon(hexGroup, new Point(30, 10), radius);

        AddHexagon(hexGroup, new Point(20, 10 + vertOffset), radius);
        AddHexagon(hexGroup, new Point(0, 10 + vertOffset), radius);
        AddHexagon(hexGroup, new Point(40, 10 + vertOffset), radius);

        AddHexagon(hexGroup, new Point(10, 10 + 2 * vertOffset), radius);
        AddHexagon(hexGroup, new Point(30, 10 + 2 * vertOffset), radius);

        var hexPen = new Pen(
            new SolidColorBrush(Color.Parse("#790000")),
            1.4)
        {
            LineJoin = PenLineJoin.Round
        };

        var hexDrawing = new GeometryDrawing
        {
            Geometry = hexGroup,
            Pen = hexPen
        };

        var group = new DrawingGroup
        {
            Children = { background, hexDrawing }
        };

        var brush = new DrawingBrush
        {
            Drawing = group,
            TileMode = TileMode.Tile,
            DestinationRect = new RelativeRect(0, 0, 40, 40, RelativeUnit.Absolute),
            Stretch = Stretch.None
        };

        return brush;
    }

    private static void AddHexagon(GeometryGroup group, Point center, double radius)
    {
        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            Point GetPoint(int i)
            {
                var angleDeg = 30 + i * 60; // 30° Offset für flat-top
                var angleRad = Math.PI / 180.0 * angleDeg;
                var x = center.X + radius * Math.Cos(angleRad);
                var y = center.Y + radius * Math.Sin(angleRad);
                return new Point(x, y);
            }

            var p0 = GetPoint(0);
            ctx.BeginFigure(p0, false);

            for (var i = 1; i < 6; ++i)
            {
                ctx.LineTo(GetPoint(i));
            }

            ctx.EndFigure(true);
        }

        group.Children.Add(geometry);
    }
}