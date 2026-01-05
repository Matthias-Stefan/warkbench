using Avalonia.Media;

namespace warkbench.viewport;

internal static class ViewportStyle
{
    internal static Color XColor = Color.FromArgb(255, 220, 60, 60);
    internal static Color YColor = Color.FromArgb(255, 60, 220, 60);
    internal static Color ZColor = Color.FromArgb(255, 60, 60, 220);
    internal static Color HandleColor = Colors.DodgerBlue;
    
    internal static readonly IBrush XBrush = new SolidColorBrush(XColor); 
    internal static readonly IBrush YBrush = new SolidColorBrush(YColor);
    internal static readonly IBrush ZBrush = new SolidColorBrush(ZColor);
    internal static readonly IBrush HandleBrush = new SolidColorBrush(HandleColor);
}