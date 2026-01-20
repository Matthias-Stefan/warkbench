using Avalonia.Media;
using Avalonia;
using System.Globalization;
using warkbench.src.ui.viewport.Common;

namespace warkbench.src.ui.viewport.Rendering;

internal class DebugRenderer
{
    public void Render(DrawingContext ctx, Camera cam, Rect viewportBounds, Avalonia.Point pointerPos)
    {
        using var _ = ctx.PushClip(viewportBounds);

        var worldPos = cam.ScreenToWorld(pointerPos, viewportBounds.Size);
        
        var cx = (int)pointerPos.X;
        var cy = (int)pointerPos.Y;
        
        var wx = Math.Round(worldPos.X, 2);
        var wy = Math.Round(worldPos.Y, 2);
                
        var text = new FormattedText(
            $"@Canvas: ({cx}, {cy}), @World: ({wx}, {wy})",
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            _glyphTypeface,
            12,
            _glyphBrush
        );
        
        ctx.DrawText(text, new Point(
            viewportBounds.BottomLeft.X - _debugTextDeltaMouseCanvas.X, 
            viewportBounds.BottomLeft.Y - _debugTextDeltaMouseCanvas.Y));
    }
    
    private readonly Typeface _glyphTypeface = new Typeface("Segoe UI");
    private readonly IBrush _glyphBrush = new SolidColorBrush(Colors.White);

    private readonly Avalonia.Point _debugTextDeltaMouseCanvas = new Point(-10, 50);
}