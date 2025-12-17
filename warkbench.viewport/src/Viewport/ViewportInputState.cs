namespace warkbench.viewport;
public sealed class ViewportInputState
{
    public void BeginPan(Avalonia.Point start)
    {
        IsPanning = true;
        LastPointerPosition = start;
    }

    public void EndPan()
    {
        IsPanning = false;
    }
    
    public bool IsPanning { get; private set; }
    public Avalonia.Point LastPointerPosition { get; set; }
}