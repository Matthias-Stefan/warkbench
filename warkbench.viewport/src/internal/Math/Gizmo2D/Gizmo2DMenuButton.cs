using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;


namespace warkbench.viewport;
/// <summary>
/// Represents a radial gizmo menu button that renders a geometry-based icon
/// positioned on a circular menu band in screen space.
/// </summary>
internal sealed class Gizmo2DMenuButton
{
    /// <summary>
    /// Creates a new gizmo menu button.
    /// </summary>
    /// <param name="iconName">Resource key of the StreamGeometry icon.</param>
    /// <param name="degrees">Angular position on the menu ring (degrees).</param>
    /// <param name="rotation">Local icon rotation (degrees).</param>
    /// <param name="scale">Uniform icon scale factor.</param>
    public Gizmo2DMenuButton(string iconName, double degrees, double rotation, double scale)
    {
        IconName = iconName;
        Degrees = degrees;
        Rotation = rotation;
        Scale = scale;
    }
    
    /// <summary>
    /// Renders the menu button icon at its radial position.
    /// </summary>
    /// <param name="ctx">Target drawing context.</param>
    /// <param name="originScreenSpace">Center of the radial menu in screen space.</param>
    /// <param name="ringRadiusScreenSpace">Radius of the menu ring in screen space.</param>
    public void Render(DrawingContext ctx, Point originScreenSpace, double ringRadiusScreenSpace)
    {
        // Resolve icon geometry from application resources
        if (Application.Current?.FindResource(IconName) is not StreamGeometry streamGeometry)
            return;

        var bounds = streamGeometry.Bounds;

        // Convert angular position to radians
        var theta = Degrees * Math.PI / 180.0;

        // Compute icon position on the ring
        var targetX = originScreenSpace.X + Math.Cos(theta) * ringRadiusScreenSpace;
        var targetY = originScreenSpace.Y + Math.Sin(theta) * ringRadiusScreenSpace;
        
        // Clone to avoid mutating shared resource geometry
        var geometry = streamGeometry.Clone();

        geometry.Transform = new TransformGroup
        {
            Children =
            {
                // Center geometry at origin for correct scale/rotation
                new TranslateTransform(
                    -(bounds.X + bounds.Width  * 0.5),
                    -(bounds.Y + bounds.Height * 0.5)),

                // Apply uniform icon scale
                new ScaleTransform(Scale, Scale),

                // Optional local icon rotation
                // new RotateTransform(Rotation),

                // Move icon to its final screen-space position
                new TranslateTransform(targetX, targetY),
            }
        };

        // Draw filled icon geometry
        var brush = ViewportStyle.Gizmo2DMenuBrush;
        if (IsPressed)
        {
            brush = ViewportStyle.Gizmo2DMenuPressedBrush;
        }
        else if (IsHovered)
        {
            brush = ViewportStyle.Gizmo2DMenuHoverBrush;
        }

        ctx.DrawGeometry(brush, null, geometry);
    }
    
    /// <summary>
    /// Performs a circular hit test against the button in screen space.
    /// </summary>
    /// <param name="pointerScreenSpace">Pointer position in screen space.</param>
    /// <param name="originScreenSpace">Center of the radial menu in screen space.</param>
    /// <param name="ringRadiusScreenSpace">Radius of the menu ring in screen space.</param>
    /// <param name="hitRadius">Hit radius around the icon center.</param>
    /// <returns>True if the pointer is inside the hit area.</returns>
    public bool HitTest(Point pointerScreenSpace, Point originScreenSpace, double ringRadiusScreenSpace, double hitRadius)
    {
        // Convert angular position to radians
        var theta = Degrees * Math.PI / 180.0;

        // Compute button center in screen space
        var center = new Point(
            originScreenSpace.X + Math.Cos(theta) * ringRadiusScreenSpace,
            originScreenSpace.Y + Math.Sin(theta) * ringRadiusScreenSpace);

        // Circular hit test (no geometry allocation)
        var dx = pointerScreenSpace.X - center.X;
        var dy = pointerScreenSpace.Y - center.Y;

        return (dx * dx + dy * dy) <= (hitRadius * hitRadius);
    }
    
    /// <summary>
    /// Updates hover state using circular hit testing.
    /// </summary>
    public void UpdateHover(Point pointerScreenSpace, Point originScreenSpace, double ringRadiusScreenSpace, double hitRadius)
    {
        IsHovered = HitTest(pointerScreenSpace, originScreenSpace, ringRadiusScreenSpace, hitRadius);
    }
    
    // Resource key of the icon geometry
    public string IconName { get; set; }

    // Angular position on the radial menu (degrees)
    public double Degrees { get; set; } = 0.0d;

    // Local rotation of the icon (degrees)
    public double Rotation { get; set; } = 0.0d;

    // Uniform scale factor of the icon
    public double Scale { get; set; }

    // True if the pointer is currently inside the button hit area
    public bool IsHovered { get; set; }

    // True while the pointer is pressed and captured by this button
    public bool IsPressed { get; set; }
}