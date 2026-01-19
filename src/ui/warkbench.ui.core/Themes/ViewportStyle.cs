using Avalonia.Media;

namespace warkbench.src.ui.core.Themes;

public static class ViewportStyle
{
    public static Color XColor = Color.FromArgb(255, 220, 60, 60);
    public static Color YColor = Color.FromArgb(255, 60, 220, 60);
    public static Color ZColor = Color.FromArgb(255, 60, 60, 220);
    public static Color HandleColor = Colors.DodgerBlue;
    public static Color Gizmo2DMenuColor = Color.FromArgb(244, 128, 128, 128);
    public static Color Gizmo2DMenuHoverColor = Color.FromArgb(224, 255, 255, 255);
    public static Color Gizmo2DMenuPressedColor = Color.FromArgb(255, 255, 255, 0);
    public static Color TooltipBorderColor = Color.FromArgb(255, 0, 0, 0);
    public static Color TooltipBackgroundColor = Color.FromArgb(255, 43, 43, 43);
    //public static Color TooltipBackgroundColor = Color.FromArgb(255, 38, 40, 43);
    
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