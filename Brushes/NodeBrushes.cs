using Avalonia.Media;

namespace warkbench.Brushes;
public static class NodeBrushes
{
    public static SolidColorBrush Vector2D  { get; } = new SolidColorBrush(Colors.SlateGray);
    public static SolidColorBrush Bool      { get; } = new SolidColorBrush(Colors.Violet);
    public static SolidColorBrush Class     { get; } = new SolidColorBrush(Colors.Yellow);
    public static SolidColorBrush Float     { get; } = new SolidColorBrush(Colors.Goldenrod);
    public static SolidColorBrush Int       { get; } = new SolidColorBrush(Colors.IndianRed);
    public static SolidColorBrush Rectangle { get; } = new SolidColorBrush(Colors.SeaGreen);
    public static SolidColorBrush String    { get; } = new SolidColorBrush(Colors.OliveDrab);
    public static SolidColorBrush Texture   { get; } = new SolidColorBrush(Colors.LightSkyBlue);
}