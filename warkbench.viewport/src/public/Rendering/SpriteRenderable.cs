using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia;
using warkbench.core;


namespace warkbench.viewport;
public sealed class SpriteRenderable : IRenderable
{
    public SpriteRenderable(Bitmap bitmap, Rect bounds, int layer, 
        Rect? sourceRect = null, float opacity = 1.0f, Color? tint = null)
    {
        Content = bitmap;
        Bounds = bounds;
        Layer = layer;
        SourceRect = sourceRect;
        Opacity = WarkbenchMath.Clamp01(opacity);
        Tint = tint;
    }

    public Bitmap Content { get; }
    
    public Rect Bounds { get; }
    public int Layer { get; }
    
    public Rect? SourceRect { get; }
    public float Opacity { get; }
    public Color? Tint { get; }
}