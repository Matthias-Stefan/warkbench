using Avalonia;
using Avalonia.Controls;
using warkbench.Models;


namespace warkbench.ViewModels;

public sealed class PropertyViewModel : GraphViewModel, IProperty
{
    public PropertyViewModel(GraphModel model) : base(model)
    {
    }
    
    public override string DetailsHeader => "Property";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_precision_manufacturing") ?? null;
}