using warkbench.src.ui.core.Math;

namespace warkbench.src.ui.viewport.Input;

internal sealed class ViewportInputState
{
    public Avalonia.Point MousePos { get; set; } = AvaloniaMathExtension.ZeroPoint;
    public Avalonia.Point LeftButtonPressedPos { get; set; } = AvaloniaMathExtension.ZeroPoint;
    public Avalonia.Point MiddleButtonPressedPos { get; set; } = AvaloniaMathExtension.ZeroPoint;
    public Avalonia.Point RightButtonPressedPos { get; set; } = AvaloniaMathExtension.ZeroPoint;

    public bool IsPanning { get; private set; } = false;
    public bool IsDragging { get; set; } = false;
}