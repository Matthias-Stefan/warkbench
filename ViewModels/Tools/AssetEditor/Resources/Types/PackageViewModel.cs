using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using warkbench.Models;


namespace warkbench.ViewModels;

public sealed partial class PackageViewModel : AssetViewModel
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

    public override string DetailsHeader => "Package";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_package") ?? null;

    public PackageModel Model { get; }
    public GraphModel Blueprint { get; }
}