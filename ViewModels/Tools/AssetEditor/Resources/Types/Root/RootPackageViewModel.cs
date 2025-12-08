namespace warkbench.ViewModels;
public class RootPackageViewModel : AssetViewModel
{
    public RootPackageViewModel(string rootVirtualPath)
    {
    }

    protected override string GetName()
    {
        return "Packages";
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
