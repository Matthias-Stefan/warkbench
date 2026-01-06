using System;
using Avalonia;
using Avalonia.Media;


namespace warkbench.core;
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
    
    /// <summary>
    /// Returns the absolute angle (in radians) of a direction vector.
    /// Angle is measured from the positive X-axis, counter-clockwise.
    /// </summary>
    public static double Atan2(Avalonia.Vector v)
        => Math.Atan2(v.Y, v.X);

    /// <summary>
    /// Returns the absolute angle (in radians) of the direction
    /// from origin to point.
    /// </summary>
    public static double Atan2(Avalonia.Point point, Avalonia.Point origin)
        => Math.Atan2(point.Y - origin.Y, point.X - origin.X);

    /// <summary>
    /// Converts an angle (radians) into a normalized direction vector.
    /// 0 rad points to the right (+X), CCW rotation.
    /// </summary>
    public static Avalonia.Vector DirectionFromAngle(double angle)
        => new Avalonia.Vector(Math.Cos(angle), Math.Sin(angle));

    /// <summary>
    /// Wraps an angle to the range [-PI, PI].
    /// Useful for stable rotation deltas.
    /// </summary>
    public static double WrapPi(double angle)
    {
        while (angle <= -Math.PI) angle += 2.0 * Math.PI;
        while (angle >  Math.PI)  angle -= 2.0 * Math.PI;
        return angle;
    }
}