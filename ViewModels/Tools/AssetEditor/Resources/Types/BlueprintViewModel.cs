using Avalonia;
using Avalonia.Controls;
using warkbench.Models;


namespace warkbench.ViewModels;

public sealed class BlueprintViewModel : GraphViewModel, IBlueprint
{
    public BlueprintViewModel(GraphModel model) : base(model)
    {
    }
    
    public override string DetailsHeader => "Blueprint";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_blueprint_package") ?? null;
}