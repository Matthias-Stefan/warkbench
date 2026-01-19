using Avalonia.Media;
using Avalonia;
using System;
using warkbench.src.ui.core.Math;

namespace warkbench.src.ui.viewport.Rendering;

public sealed class LineRenderable : IRenderable
{
    public LineRenderable(Avalonia.Point startPoint, Avalonia.Point endPoint, int layer, Color color,
        float thickness = 1.0f, float opacity = 1.0f, PenLineCap penLineCap = PenLineCap.Round, PenLineJoin penLineJoin = PenLineJoin.Round)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;

        Layer = layer;

        Color = color;
        Thickness = thickness <= 0.0f ? 1.0f : thickness;
        Opacity = AvaloniaMathExtension.Clamp01(opacity);

        LineCap = penLineCap;
        LineJoin = penLineJoin;

        Bounds = ComputeBounds(StartPoint, EndPoint, Thickness);
    }

    private static Rect ComputeBounds(Point a, Point b, float thickness)
    {
        var x1 = Math.Min(a.X, b.X);
        var y1 = Math.Min(a.Y, b.Y);
        var x2 = Math.Max(a.X, b.X);
        var y2 = Math.Max(a.Y, b.Y);

        var pad = Math.Max(1.0, thickness) * 0.5;
        return new Rect(x1 - pad, y1 - pad, (x2 - x1) + pad * 2.0, (y2 - y1) + pad * 2.0);
    }
    
    public Point StartPoint { get; }
    public Point EndPoint { get; }
    
    public int Layer { get; }
    public Rect Bounds { get; }
    
    public Color Color { get; }
    public float Thickness { get; }
    public float Opacity { get; }
    public PenLineCap LineCap { get; }
    public PenLineJoin LineJoin { get; }
}