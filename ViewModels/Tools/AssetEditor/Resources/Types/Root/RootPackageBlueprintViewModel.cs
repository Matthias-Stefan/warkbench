using Avalonia;
using Avalonia.Controls;

namespace warkbench.ViewModels;

public class RootPackageBlueprintViewModel : AssetViewModel
{
    public RootPackageBlueprintViewModel(string rootVirtualPath)
    {
    }

    protected override string GetName()
    {
        return "Blueprints";
    }

    protected override void SetName(string value)
    {
    }

    protected override string GetVirtualPath()
    {
        return string.Empty;
    }

    protected override void SetVirtualPath(string value)
    {
    }
    
    public override string DetailsHeader => GetName();
    public override object? DetailsIcon => Application.Current?.FindResource("icon_blueprint_package") ?? null;
}