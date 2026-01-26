using Avalonia.Media;

namespace warkbench.src.ui.core.Themes;

public static class WarkbenchStyle
{
    // --- Colors ---
    public readonly static Color BondiBlueColor     = Color.FromArgb(255, 9, 144, 179);
    public readonly static Color SunflowerGoldColor = Color.FromArgb(255, 240, 176, 22);
    public readonly static Color PacificBlueColor   = Color.FromArgb(255, 5, 190, 216);
    public readonly static Color JetBlackColor      = Color.FromArgb(255, 11, 36, 35);
    public readonly static Color ElectricRoseColor  = Color.FromArgb(255, 243, 1, 173);
    public readonly static Color PurpleColor        = Color.FromArgb(255, 124, 17, 120);
    public readonly static Color BananaCreamColor   = Color.FromArgb(255, 246, 230, 111);

    // --- Brushes --- 
    public static readonly IBrush BondiBlueBrush =
        new SolidColorBrush(BondiBlueColor);

    public static readonly IBrush SunflowerGoldBrush =
        new SolidColorBrush(SunflowerGoldColor);

    public static readonly IBrush PacificBlueBrush =
        new SolidColorBrush(PacificBlueColor);

    public static readonly IBrush JetBlackBrush =
        new SolidColorBrush(JetBlackColor);

    public static readonly IBrush ElectricRoseBrush =
        new SolidColorBrush(ElectricRoseColor);

    public static readonly IBrush PurpleBrush =
        new SolidColorBrush(PurpleColor);

    public static readonly IBrush BananaCreamBrush =
        new SolidColorBrush(BananaCreamColor);
}