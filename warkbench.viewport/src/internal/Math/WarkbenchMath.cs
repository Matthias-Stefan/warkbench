using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
public static class WarkbenchMath
{
    /// <summary>
    /// Represents an empty rectangle with zero size at the origin.
    /// </summary>
    public static readonly Avalonia.Rect EmptyRect = new();

    /// <summary>
    /// Represents the point at the origin (0, 0).
    /// </summary>
    public static readonly Avalonia.Point ZeroPoint = new(0, 0);
    
    /// <summary>
    /// Represents an empty size (0 × 0).
    /// </summary>
    public static readonly Avalonia.Size EmptySize = new(0, 0);
    
    /// <summary>
    /// Represents an empty geometry (zero-size, non-rendering).
    /// </summary>
    public static readonly Geometry EmptyGeometry =
        new RectangleGeometry(new Rect(ZeroPoint, EmptySize));
    
    /// <summary>
    /// Clamps the given value to the range [0, 1].
    /// </summary>
    /// <param name="value">
    /// The value to clamp.
    /// </param>
    /// <returns>
    /// The clamped value, guaranteed to be between 0 and 1.
    /// </returns>
    public static float Clamp01(float value) => value < 0f ? 0f : (value > 1f ? 1f : value); 
}