namespace warkbench.viewport;
public class ViewportCamera
{
    public Avalonia.Point ScreenToWorld(Avalonia.Point screen, Avalonia.Size viewportSize)
    {
        var center = new Avalonia.Vector(viewportSize.Width * 0.5,  viewportSize.Height * 0.5);
        var v = (Avalonia.Vector)screen - center - Pan;
        return (Avalonia.Point)(v / Zoom);
    }

    public Avalonia.Point WorldToScreen(Avalonia.Point world, Avalonia.Size viewportSize)
    {
        var center = new Avalonia.Vector(viewportSize.Width * 0.5, viewportSize.Height * 0.5);
        var v = ((Avalonia.Vector)world * Zoom) + center + Pan;
        return (Avalonia.Point)v;
    }
    
    public void PanByPixels(Avalonia.Vector deltaPixels)
    {
        Pan += deltaPixels;
    }
    
    public void ZoomAt(Avalonia.Point screenAnchor, double zoomFactor, Avalonia.Size viewportSize)
    {
        var oldZoom = Zoom;
        var newZoom = System.Math.Clamp(oldZoom * zoomFactor, MinZoom, MaxZoom);
        if (System.Math.Abs(newZoom - oldZoom) < 1e-9)
            return;

        var center = new Avalonia.Vector(viewportSize.Width * 0.5, viewportSize.Height * 0.5);
        var pre = (Avalonia.Vector)screenAnchor - center - Pan;
        Zoom = newZoom;
        var post = pre * (newZoom / oldZoom);
        Pan += pre - post;
    }

    public double Zoom { get; private set; } = 1.0;
    
    public double MinZoom { get; set; } = 0.10;
    public double MaxZoom { get; set; } = 10.0;

    public Avalonia.Vector Pan { get; private set; } = Avalonia.Vector.Zero;
}