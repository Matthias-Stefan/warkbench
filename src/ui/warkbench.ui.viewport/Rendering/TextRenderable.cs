using Avalonia.Media;
using Avalonia;
using warkbench.src.ui.core.Math;

namespace warkbench.src.ui.viewport.Rendering;

public sealed class TextRenderable : IRenderable
{
    public TextRenderable(string text, Point position, int layer, Color color,
        float fontSize = 12.0f, string fontFamily = "Segoe UI", FontWeight? fontWeight = null, FontStyle? fontStyle = null, float opacity = 1.0f)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Position = position;

        Layer = layer;

        Color = color;
        FontSize = fontSize <= 0.0f ? 12.0f : fontSize;
        FontFamily = fontFamily;
        FontWeight = fontWeight ?? FontWeight.Normal;
        FontStyle = fontStyle ?? FontStyle.Normal;
        Opacity = AvaloniaMathExtension.Clamp01(opacity);

        Bounds = ComputeBounds(Position, Text, FontSize);
    }

    public string Text { get; }
    public Point Position { get; }

    public int Layer { get; }
    public Rect Bounds { get; }

    public Color Color { get; }
    public float FontSize { get; }
    public string FontFamily { get; }
    public FontWeight FontWeight { get; }
    public FontStyle FontStyle { get; }
    public float Opacity { get; }

    private static Rect ComputeBounds(Point pos, string text, float size)
    {
        var width = Math.Max(1.0, text.Length * size * 0.6);
        var height = size * 1.2;
        return new Rect(pos.X, pos.Y, width, height);
    }
}