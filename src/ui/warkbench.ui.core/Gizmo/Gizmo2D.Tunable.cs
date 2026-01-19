

using System.Collections.Generic;

namespace warkbench.src.ui.core.Gizmo;

public partial class Gizmo2D
{
    public static List<double> ModifierValues { get; } = [ 3.0, 2.0, 1.0, 0.5, 0.25 ];
    public static double ModifierOffset { get; set; } = 30d;
    public static double ModifierStep { get; set; } = 60d;
    
    public static double AxisHitHalfThickness { get; set; } = 6d;
    public static double CenterHandleSize { get; set; } = 8d;
    public static double HoverThickness  { get; set; } = 4d;
    public static double StrokeThickness { get; set; } = 2d;
    
    public static double InnerScaleLength { get; set; } = 18d;
    public static double InnerScaleOffset { get; set; } = 14d;
    public static double ScaleHandleSize  { get; set; } = 8d;
    
    public static double OuterTranslateLength { get; set; } = 30d;
    public static double OuterTranslateOffset { get; set; } = 18d;
    
    public static double IndicatorLength { get; set; } = 10d;
    public static double RotateHitBand { get; set; } = 8d;
    public static double RotateRadius { get; set; } = 52d;
    
    public static double MenuOffset { get; set; } = 12d;
    public static double MenuButtonHitRadius { get; set; } = 12d;
}