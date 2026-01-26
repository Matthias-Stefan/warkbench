using Avalonia.Media;

namespace warkbench.src.ui.core.Themes;

public static class ViewportStyle
{
    // --- Colors ---
    public readonly static Color XColor = Color.FromArgb(255, 220, 60, 60);
    public readonly static Color YColor = Color.FromArgb(255, 60, 220, 60);
    public readonly static Color ZColor = Color.FromArgb(255, 60, 60, 220);
    public readonly static Color HandleColor = Colors.DodgerBlue;
    public readonly static Color Gizmo2DMenuColor = Color.FromArgb(244, 128, 128, 128);
    public readonly static Color Gizmo2DMenuHoverColor = Color.FromArgb(224, 255, 255, 255);
    public readonly static Color Gizmo2DMenuPressedColor = Color.FromArgb(255, 255, 255, 0);
    public readonly static Color TooltipBorderColor = Color.FromArgb(255, 0, 0, 0);
    public readonly static Color TooltipBackgroundColor = Color.FromArgb(255, 43, 43, 43);
    
    // --- Brushes --- 
    public static readonly IBrush XBrush = new SolidColorBrush(XColor); 
    public static readonly IBrush YBrush = new SolidColorBrush(YColor);
    public static readonly IBrush ZBrush = new SolidColorBrush(ZColor);
    public static readonly IBrush HandleBrush = new SolidColorBrush(HandleColor);
    public static readonly IBrush Gizmo2DMenuBrush = new SolidColorBrush(Gizmo2DMenuColor);
    public static readonly IBrush Gizmo2DMenuHoverBrush = new SolidColorBrush(Gizmo2DMenuHoverColor);
    public static readonly IBrush Gizmo2DMenuPressedBrush = new SolidColorBrush(Gizmo2DMenuPressedColor);
    public static readonly IBrush TooltipBorderBrush = new SolidColorBrush(TooltipBorderColor);
    public static readonly IBrush TooltipBackgroundBrush = new SolidColorBrush(TooltipBackgroundColor);
}