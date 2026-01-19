using Avalonia.Controls;
using Avalonia;
using System;
using warkbench.src.basis.interfaces.Worlds;


namespace warkbench.ViewModels;

public sealed partial class WorldViewModel : AssetViewModel
{
    public WorldViewModel(IWorld model)
    {
        Model = model;
    }
    
    protected override string GetName()
    {
        return Model.Name;
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
    
    public override string DetailsHeader => "World";
    public override object? DetailsIcon => Application.Current?.FindResource("icon_globe") ?? null;
    
    public Guid Guid => Model.Id;
    public int TileSize => Model.TileSize;
    public int ChunkSize => Model.ChunkResolution;
    
    public IWorld Model { get; }
}