namespace warkbench.ViewModels;
public class RootPropertiesViewModel : AssetViewModel
{
    public RootPropertiesViewModel(string rootVirtualPath)
    {
    }

    protected override string GetName()
    {
        return "Properties";
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
