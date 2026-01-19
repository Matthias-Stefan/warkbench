using Avalonia;
using Avalonia.Controls;

namespace warkbench.ViewModels;

public class RootWorldViewModel : AssetViewModel
{
    public RootWorldViewModel(string rootVirtualPath)
    {
        
    }

    protected override string GetName()
    {
        return "Worlds";
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
    public override object? DetailsIcon => Application.Current?.FindResource("icon_globe") ?? null;
}