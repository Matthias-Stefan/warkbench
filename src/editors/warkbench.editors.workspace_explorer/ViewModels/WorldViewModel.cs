using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.core.Models;
using warkbench.src.editors.core.ViewModel;

namespace warkbench.src.editors.workspace_explorer.ViewModels;

public partial class WorldViewModel(LocalPath localPath, IWorld? world = null, LoadState? loadState = null)
    : TreeNodePayload(world?.Name ?? string.Empty, world, loadState)
{
    public void AttachWorld(IWorld world, LoadState loadState)
    {
        _world = world;
        LoadState = loadState;
    }
    
    [ObservableProperty] private LocalPath _localPath = localPath;
    
    [ObservableProperty] private LoadState? _loadState = loadState;
    
    public Guid Id => _world?.Id ?? Guid.Empty;
    public bool IsDirty => _world?.IsDirty ?? false;
    public string Description => _world?.Description ?? string.Empty;
    public DateTime CreatedAt => _world?.CreatedAt ?? DateTime.MinValue;
    public DateTime LastModifiedAt => _world?.LastModifiedAt ?? DateTime.MinValue;
    public int TileSize => _world?.TileSize ?? 0;
    public int ChunkResolution => _world?.ChunkResolution ?? 0;
    public int TotalTilesPerChunk => _world?.TotalTilesPerChunk ?? 0;
    
    private IWorld? _world = world;
}