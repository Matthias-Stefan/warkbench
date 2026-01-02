using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;


namespace warkbench.viewport;
internal class SelectionRenderer
{
    public void Render(DrawingContext ctx, ViewportCamera cam, Rect viewportBounds, Rect selectionBounds)
    {
        using var _ = ctx.PushClip(viewportBounds);
        
        var x = Math.Floor(selectionBounds.X) + 0.5;
        var y = Math.Floor(selectionBounds.Y) + 0.5;
        var w = Math.Max(0, Math.Round(selectionBounds.Width) - 1.0);
        var h = Math.Max(0, Math.Round(selectionBounds.Height) - 1.0);
        
        var snapped = new Rect(x, y, w, h);
        ctx.DrawRectangle(_background, _border, snapped, 20f);
    }
    
    private readonly IPen _border = new Pen(new SolidColorBrush(Colors.DodgerBlue), 1);
    private readonly IBrush _background = new SolidColorBrush(Color.FromArgb(40, 0x1E, 0x90, 0xFF));
}