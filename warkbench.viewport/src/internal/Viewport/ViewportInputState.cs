using warkbench.core;


namespace warkbench.viewport;
internal sealed class ViewportInputState
{
    public Avalonia.Point MousePos { get; set; } = WarkbenchMath.ZeroPoint;
    public Avalonia.Point LeftButtonPressedPos { get; set; } = WarkbenchMath.ZeroPoint;
    public Avalonia.Point MiddleButtonPressedPos { get; set; } = WarkbenchMath.ZeroPoint;
    public Avalonia.Point RightButtonPressedPos { get; set; } = WarkbenchMath.ZeroPoint;

    public bool IsPanning { get; private set; } = false;
    public bool IsDragging { get; set; } = false;
}