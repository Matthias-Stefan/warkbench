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
}
