using System;
using Avalonia.Media;
using Avalonia;
using System.Globalization;
using System.Net.Cache;


namespace warkbench.viewport;
internal class DebugRenderer
{
    public void Render(DrawingContext ctx, ViewportCamera cam, Rect viewportBounds, Avalonia.Point pointerPos)
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
        
        
        // Begin Gizmo Test
        IBrush background = new SolidColorBrush(Color.Parse("#191a1c"));
        
        // Rotation: Main Handle
        _rotationHandle.Transform = new TranslateTransform(150, 150);
        _selRotationHandle.Transform = new TranslateTransform(150, 150);
        
        if (_selRotationHandle.FillContains(pointerPos))
        {
            ctx.DrawGeometry(new SolidColorBrush(Colors.Yellow), null, _rotationHandle);
            
        }
        else
        {
            //ctx.DrawGeometry(new SolidColorBrush(Colors.IndianRed), null, _scaleHandleX);
            //ctx.DrawGeometry(new SolidColorBrush(Colors.SeaGreen), null, _scaleHandleY);
            
            ctx.DrawGeometry(new SolidColorBrush(Colors.DodgerBlue), null, _rotationHandle);
        }


        //ctx.DrawEllipse(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.DodgerBlue), 3), new Point(300, 300), 50, 50);
        //ctx.DrawEllipse(new SolidColorBrush(Colors.Transparent), new Pen(new SolidColorBrush(Colors.DodgerBlue), 3), new Point(300, 250), 12, 12);

        // Rotation: Reset and Default handle, Empty when 0° filled other



    }
    
    private readonly Typeface _glyphTypeface = new Typeface("Segoe UI");
    private readonly IBrush _glyphBrush = new SolidColorBrush(Colors.White);

    private readonly Avalonia.Point _debugTextDeltaMouseCanvas = new Point(-10, 50);
    
    private readonly Geometry _rotationHandle = Geometry.Parse("M 340 250 C 340 299.706 299.706 340 250 340 C 200.294 340 160 299.706 160 250 C 160 208.881 187.574 174.204 225.24 163.449 C 225.142 164.154 225.073 164.869 225.035 165.592 C 188.594 176.353 162 210.071 162 250 C 162 298.601 201.399 338 250 338 C 298.601 338 338 298.601 338 250 C 338 210.071 311.407 176.353 274.965 165.592 C 274.927 164.869 274.858 164.155 274.76 163.449 C 312.426 174.204 340 208.882 340 250 Z M 265 166.924 C 265 175.208 258.284 181.924 250 181.924 C 241.716 181.924 235 175.208 235 166.924 C 235 158.64 241.716 151.924 250 151.924 C 258.284 151.924 265 158.64 265 166.924 Z M 250 156.924 C 244.477 156.924 240 161.401 240 166.924 C 240 172.447 244.477 176.924 250 176.924 C 255.523 176.924 260 172.447 260 166.924 C 260 161.401 255.523 156.924 250 156.924 Z");
    private readonly Geometry _selRotationHandle = Geometry.Parse("M 345 250 C 345 302.467 302.467 345 250 345 C 197.533 345 155 302.467 155 250 C 155 205.577 185.49 168.276 226.685 157.882 C 225.597 160.686 225 163.735 225 166.924 C 225 168.222 225.099 169.496 225.289 170.74 C 191.517 181.258 167 212.765 167 250 C 167 295.84 204.16 333 250 333 C 295.84 333 333 295.84 333 250 C 333 212.766 308.483 181.258 274.711 170.74 C 274.902 169.496 275 168.221 275 166.924 C 275 163.735 274.403 160.686 273.315 157.882 C 314.51 168.276 345 205.577 345 250 Z M 265 166.924 C 265 175.208 258.284 181.924 250 181.924 C 241.716 181.924 235 175.208 235 166.924 C 235 158.64 241.716 151.924 250 151.924 C 258.284 151.924 265 158.64 265 166.924 Z");
}