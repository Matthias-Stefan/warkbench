using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Input;
using warkbench.Models;


namespace warkbench.ViewModels;
public sealed partial class PackageViewModel : BasePackageViewModel
{
    public PackageViewModel(PackageModel model, GraphModel blueprint)
    {
        Model = model;
        Blueprint = blueprint;
    }

    protected override string GetName()
    {
        return Model.Name;
    }

    protected override void SetName(string value)
    {
        Model.Name = value;
    }

    protected override string GetVirtualPath()
    {
        return string.Empty;  
    }

    protected override void SetVirtualPath(string value)
    {
    }

    public PackageModel Model { get; }
    public GraphModel Blueprint { get; }
}