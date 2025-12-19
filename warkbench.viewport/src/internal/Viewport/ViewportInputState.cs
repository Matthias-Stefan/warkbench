using Avalonia;

namespace warkbench.viewport;
internal sealed class ViewportInputState
{
    public void LeftButtonPressed(Avalonia.Point leftButtonPosition)
    {
        LeftButtonPos = leftButtonPosition;
        LeftButtonPressedPos = leftButtonPosition;
    }
    
    public void LeftButtonReleased()
    {
        LeftButtonPos = ZeroPoint;
        LeftButtonPressedPos = ZeroPoint;
    }

    public void MiddleButtonPressed(Avalonia.Point middleButtonPosition)
    {
        MiddleButtonPos = middleButtonPosition;
        MiddleButtonPressedPos = middleButtonPosition;
        IsPanning = true;
    }
    
    public void MiddleButtonReleased()
    {
        MiddleButtonPos = ZeroPoint;
        MiddleButtonPressedPos = ZeroPoint;
        IsPanning = false;
    }

    public void RightButtonPressed(Avalonia.Point rightButtonPosition)
    {
        RightButtonPos = rightButtonPosition;
        RightButtonPressedPos = rightButtonPosition;
    }

    public void RightButtonReleased()
    {
        RightButtonPos = ZeroPoint;
        RightButtonPressedPos = ZeroPoint;
    }
    
    public void EndPan()
    {
        IsPanning = false;
    }

    public Avalonia.Point LeftButtonPressedPos { get; set; }
    public Avalonia.Point LeftButtonPos { get; set; }
    
    public Avalonia.Point MiddleButtonPressedPos { get; set; }
    public Avalonia.Point MiddleButtonPos { get; set; }
    
    public Avalonia.Point RightButtonPressedPos { get; set; }
    public Avalonia.Point RightButtonPos { get; set; }
    
    public bool IsPanning { get; private set; }
    public bool IsDragging { get; set; }
    
    private static readonly Avalonia.Rect EmptyRect = new();
    private static readonly Avalonia.Point ZeroPoint = new(0, 0);
}