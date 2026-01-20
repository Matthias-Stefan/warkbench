using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using System.Globalization;
using warkbench.src.ui.core.Themes;

namespace warkbench.src.ui.core.Gizmo;

/// <summary>
/// Represents a radial gizmo menu button that renders a geometry-based icon
/// positioned on a circular menu band in screen space.
/// </summary>
internal class Gizmo2DMenuButton
{
    /// <summary>Creates a new gizmo menu button.</summary>
    /// <param name="iconName">Resource key of the StreamGeometry icon.</param>
    /// <param name="tooltip">Tooltip text shown when the button is hovered.</param>
    /// <param name="degrees">Angular position on the menu ring (degrees).</param>
    /// <param name="rotation">Local icon rotation (degrees).</param>
    /// <param name="scale">Uniform icon scale factor.</param>
    public Gizmo2DMenuButton(string iconName, string tooltip, double degrees, double rotation, double scale)
    {
        IconName  = iconName;
        Tooltip   = tooltip;
        Degrees   = degrees;
        Rotation  = rotation;
        Scale     = scale;
    }

    /// <summary>
    /// Renders the menu button icon at its radial position.
    /// </summary>
    /// <param name="ctx">Target drawing context.</param>
    /// <param name="originScreenSpace">Center of the radial menu in screen space.</param>
    /// <param name="mousePos">Mouse position in screen space.</param>
    /// <param name="ringRadiusScreenSpace">Radius of the menu ring in screen space.</param>
    public void Render(DrawingContext ctx, Point originScreenSpace, Point mousePos, double ringRadiusScreenSpace)
    {
        if (Application.Current?.FindResource(IconName) is not StreamGeometry streamGeometry)
            return;

        var (center, theta) = GetCenterAndAngle(originScreenSpace, ringRadiusScreenSpace);

        // Clone to avoid mutating shared resource geometry
        var geometry = streamGeometry.Clone();
        geometry.Transform = BuildIconTransform(streamGeometry.Bounds, center);

        ctx.DrawGeometry(GetIconBrush(), null, geometry);

        if (ShouldShowTooltip && !string.IsNullOrWhiteSpace(Tooltip))
            RenderTooltip(ctx, mousePos);
    }

    /// <summary>
    /// Performs a circular hit test against the button in screen space.
    /// </summary>
    public bool HitTest(Point pointerScreenSpace, Point originScreenSpace, double ringRadiusScreenSpace, double hitRadius)
    {
        var (center, _) = GetCenterAndAngle(originScreenSpace, ringRadiusScreenSpace);

        var dx = pointerScreenSpace.X - center.X;
        var dy = pointerScreenSpace.Y - center.Y;

        return (dx * dx + dy * dy) <= (hitRadius * hitRadius);
    }

    /// <summary>Updates hover state using circular hit testing.</summary>
    public void UpdateHover(Point pointerScreenSpace, Point originScreenSpace, double ringRadiusScreenSpace, double hitRadius)
    {
        IsHovered = HitTest(pointerScreenSpace, originScreenSpace, ringRadiusScreenSpace, hitRadius);
    }

    private (Point Center, double Theta) GetCenterAndAngle(Point originScreenSpace, double ringRadiusScreenSpace)
    {
        var theta = Degrees * System.Math.PI / 180.0;

        var center = new Point(
            originScreenSpace.X + System.Math.Cos(theta) * ringRadiusScreenSpace,
            originScreenSpace.Y + System.Math.Sin(theta) * ringRadiusScreenSpace);

        return (center, theta);
    }

    private Transform BuildIconTransform(Rect bounds, Point center)
    {
        var halfW = bounds.Width * 0.5;
        var halfH = bounds.Height * 0.5;

        return new TransformGroup
        {
            Children =
            {
                // Center geometry at origin for correct scale/rotation
                new TranslateTransform(-(bounds.X + halfW), -(bounds.Y + halfH)),

                // Apply uniform icon scale
                new ScaleTransform(Scale, Scale),

                // Optional local icon rotation
                // new RotateTransform(Rotation),

                // Move icon to its final screen-space position
                new TranslateTransform(center.X, center.Y),
            }
        };
    }

    private IBrush GetIconBrush()
    {
        if (IsPressed)
            return ViewportStyle.Gizmo2DMenuPressedBrush;

        if (IsHovered)
            return ViewportStyle.Gizmo2DMenuHoverBrush;

        return ViewportStyle.Gizmo2DMenuBrush;
    }

    private void RenderTooltip(DrawingContext ctx, Point mousePos)
    {
        var ft = new FormattedText(
            Tooltip,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            12,
            new SolidColorBrush(Colors.White));

        const double padding = 8d;
        const double mouseOffset = 18d;

        var rect = new Rect(
            new Point(mousePos.X, mousePos.Y + mouseOffset),
            new Size(ft.Width + 2 * padding, ft.Height + 2 * padding));

        ctx.DrawRectangle(
            ViewportStyle.TooltipBackgroundBrush,
            new Pen(ViewportStyle.TooltipBorderBrush, 0.5d),
            rect,
            4,
            4);

        ctx.DrawText(ft, new Point(mousePos.X + padding, mousePos.Y + padding + mouseOffset));
    }

    /// <summary>Resource key of the icon geometry.</summary>
    public virtual string IconName { get; }

    /// <summary>Tooltip text (may be overridden by stateful buttons).</summary>
    public virtual string Tooltip { get; }

    /// <summary>Angular position on the radial menu (degrees).</summary>
    public double Degrees { get; } = 0.0d;

    /// <summary>Local rotation of the icon (degrees).</summary>
    public double Rotation { get; } = 0.0d;

    /// <summary>Uniform scale factor of the icon.</summary>
    public virtual double Scale { get; }

    /// <summary>True if the pointer is currently inside the button hit area.</summary>
    public bool IsHovered
    {
        get => _isHovered;
        set
        {
            _isHovered = value;
            if (!value)
                ShouldShowTooltip = false;
        }
    }

    /// <summary>
    /// True while the pointer is pressed and captured by this button.
    /// Note: this fires OnPressed only on the rising edge (false -> true).
    /// </summary>
    public virtual bool IsPressed
    {
        get => _isPressed;
        set
        {
            var wasPressed = _isPressed;
            _isPressed = value;

            if (!wasPressed && value)
                OnPressed?.Invoke();
        }
    }

    /// <summary>Optional press callback (usually set by the owning menu/controller).</summary>
    public Action? OnPressed { get; set; }

    /// <summary>When true, the tooltip will be rendered at the mouse position.</summary>
    public bool ShouldShowTooltip { get; set; } = false;

    private bool _isHovered = false;
    private bool _isPressed = false;
}
