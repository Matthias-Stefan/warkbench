using Avalonia.Media;

namespace warkbench.viewport;

internal static class ViewportStyle
{
    internal static Color XColor = Color.FromArgb(255, 220, 60, 60);
    internal static Color YColor = Color.FromArgb(255, 60, 220, 60);
    internal static Color ZColor = Color.FromArgb(255, 60, 60, 220);
    internal static Color HandleColor = Colors.DodgerBlue;
    internal static Color Gizmo2DMenuColor = Color.FromArgb(192, 128, 128, 128);
    internal static Color Gizmo2DMenuHoverColor = Color.FromArgb(192, 255, 255, 255);
    internal static Color Gizmo2DMenuPressedColor = Color.FromArgb(255, 255, 255, 0);
    
    internal static readonly IBrush XBrush = new SolidColorBrush(XColor); 
    internal static readonly IBrush YBrush = new SolidColorBrush(YColor);
    internal static readonly IBrush ZBrush = new SolidColorBrush(ZColor);
    internal static readonly IBrush HandleBrush = new SolidColorBrush(HandleColor);
    internal static readonly IBrush Gizmo2DMenuBrush = new SolidColorBrush(Gizmo2DMenuColor);
    internal static readonly IBrush Gizmo2DMenuHoverBrush = new SolidColorBrush(Gizmo2DMenuHoverColor);
    internal static readonly IBrush Gizmo2DMenuPressedBrush = new SolidColorBrush(Gizmo2DMenuPressedColor);
}