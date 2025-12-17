using System;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;

public sealed class PackageItemViewModel : GraphViewModel, IDisposable
{
    public PackageItemViewModel(GraphModel model, GraphModel blueprint) : base(model)
    { 
        _blueprint = blueprint;
        foreach (var node in Model.Nodes)
        {
            node.NodeHeaderBrushType = NodeHeaderBrushType.None;
        }
    }

    public void Dispose()
    {
    }

    private readonly GraphModel _blueprint;
}