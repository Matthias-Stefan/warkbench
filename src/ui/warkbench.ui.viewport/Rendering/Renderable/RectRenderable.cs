using Avalonia.Media;
using Avalonia;
using warkbench.src.ui.core.Math;

namespace warkbench.src.ui.viewport.Rendering;

public class RectRenderable
{
    public RectRenderable(Rect bounds, int layer, Color fillColor,
        float fillOpacity = 1.0f, Color? strokeColor = null, float strokeThickness = 0.0f, float strokeOpacity = 1.0f, float cornerRadius = 0.0f)
    {
        Bounds = bounds;
        Layer = layer;

        FillColor = fillColor;
        FillOpacity = AvaloniaMathExtension.Clamp01(fillOpacity);

        StrokeColor = strokeColor;
        StrokeThickness = strokeThickness < 0f ? 0f : strokeThickness;
        StrokeOpacity = AvaloniaMathExtension.Clamp01(strokeOpacity);

        CornerRadius = cornerRadius < 0f ? 0f : cornerRadius;
    }
    
    
    public Rect Bounds { get; }
    public int Layer { get; }
    
    public Color FillColor { get; }
    public float FillOpacity { get; }
    public Color? StrokeColor { get; }
    public float StrokeThickness { get; }
    public float StrokeOpacity { get; }
    public float CornerRadius { get; }
    public bool HasStroke => StrokeColor.HasValue && StrokeThickness > 0.0f;
}