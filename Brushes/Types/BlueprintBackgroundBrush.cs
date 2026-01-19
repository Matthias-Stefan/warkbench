using Avalonia;
using Avalonia.Media;


namespace warkbench.Brushes;

public static class BlueprintBackgroundBrush
{
    public static IBrush Get()
    {
        var finePen = new Pen(new SolidColorBrush(Color.Parse("#33FFFFFF")), 0.5);
        var majorPen = new Pen(new SolidColorBrush(Color.Parse("#44FFFFFF")), 1);

        var background = new GeometryDrawing
        {
            Geometry = new RectangleGeometry(new Rect(0, 0, 40, 40)),
            Brush = new SolidColorBrush(Color.Parse("#0d3772"))
        };

        var fineLines = new GeometryGroup
        {
            Children =
            {
                new LineGeometry(new Point(8,0), new Point(8,40)),
                new LineGeometry(new Point(16,0), new Point(16,40)),
                new LineGeometry(new Point(24,0), new Point(24,40)),
                new LineGeometry(new Point(32,0), new Point(32,40)),
                new LineGeometry(new Point(0,8), new Point(40,8)),
                new LineGeometry(new Point(0,16), new Point(40,16)),
                new LineGeometry(new Point(0,24), new Point(40,24)),
                new LineGeometry(new Point(0,32), new Point(40,32))
            }
        };

        var fineGrid = new GeometryDrawing
        {
            Geometry = fineLines,
            Pen = finePen
        };

        var majorLines = new GeometryGroup
        {
            Children =
            {
                new LineGeometry(new Point(40,0), new Point(40,40)),
                new LineGeometry(new Point(0,40), new Point(40,40))
            }
        };

        var majorGrid = new GeometryDrawing
        {
            Geometry = majorLines,
            Pen = majorPen
        };

        var group = new DrawingGroup
        {
            Children = { background, fineGrid, majorGrid }
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
}