using Avalonia.Media;


namespace warkbench.Brushes;
public static class NodeBrushes
{
    public static SolidColorBrush Bool        { get; } = new SolidColorBrush(Color.Parse("#7ED321"));
    public static SolidColorBrush Class       { get; } = new SolidColorBrush(Color.Parse("#50E3C2"));
    public static SolidColorBrush Float       { get; } = new SolidColorBrush(Color.Parse("#BD10E0"));
    public static SolidColorBrush Int32       { get; } = new SolidColorBrush(Color.Parse("#C4515F"));
    public static SolidColorBrush Int64       { get; } = new SolidColorBrush(Color.Parse("#C4515F"));
    public static SolidColorBrush Property    { get; } = new SolidColorBrush(Colors.Crimson);
    public static SolidColorBrush Rectangle   { get; } = new SolidColorBrush(Color.Parse("#4A90E2"));
    public static SolidColorBrush String      { get; } = new SolidColorBrush(Color.Parse("#F8E71C"));
    public static SolidColorBrush Texture     { get; } = new SolidColorBrush(Color.Parse("#B8E986"));
    public static SolidColorBrush Vector2D    { get; } = new SolidColorBrush(Color.Parse("#F5A623"));
    
    
    public static IBrush BlueprintBackground { get; } = BlueprintBackgroundBrush.Get();
    
    public static IBrush PropertyBackground   { get; } = PropertyBackgroundBrush.Get();
}

