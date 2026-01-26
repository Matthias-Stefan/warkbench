using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.Models;

namespace warkbench.src.editors.workspace_explorer.ViewModels;

public partial class WorldViewModel : ObservableObject
{
    public WorldViewModel(LocalPath localPath, IWorld? world = null)
    {
        LocalPath = localPath;
        _world = world;
    }

    public void AttachWorld(IWorld world)
    {
        _world = world;
    }
    
    [ObservableProperty]
    private LoadState _loadState = LoadState.NotLoaded;
    
    public Guid Id => _world?.Id ?? Guid.Empty;
    public string Name => _world?.Name ?? string.Empty;
    public bool IsDirty => _world?.IsDirty ?? false;
    public string Description => _world?.Description ?? string.Empty;
    public DateTime CreatedAt => _world?.CreatedAt ?? DateTime.MinValue;
    public DateTime LastModifiedAt => _world?.LastModifiedAt ?? DateTime.MinValue;
    public LocalPath LocalPath { get; }
    public int TileSize => _world?.TileSize ?? 0;
    public int ChunkResolution => _world?.ChunkResolution ?? 0;
    public int TotalTilesPerChunk => _world?.TotalTilesPerChunk ?? 0;

    private IWorld? _world;
}