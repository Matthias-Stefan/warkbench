

using System.Collections.Generic;

namespace warkbench.viewport;
internal partial class Gizmo2D
{
    public double AxisHitHalfThickness { get; set; } = 6d;
    public double CenterHandleSize { get; set; } = 8d;
    public double HoverThickness  { get; set; } = 4d;
    public double IndicatorLength { get; set; } = 10d;
    public double InnerScaleLength { get; set; } = 18d;
    public double InnerScaleOffset { get; set; } = 14d;
    public double ModifierOffset { get; set; } = 30d;
    public double ModifierStep { get; set; } = 60d;
    public double OuterTranslateLength { get; set; } = 30d;
    public double OuterTranslateOffset { get; set; } = 18d;
    public double RotateHitBand { get; set; } = 8d;
    public double RotateRadius { get; set; } = 52d;
    public double ScaleHandleSize  { get; set; } = 8d;
    public double StrokeThickness { get; set; } = 2d;
    public List<double> ModifierValues { get; } = [ 3.0, 2.0, 1.0, 0.5, 0.25 ];
}