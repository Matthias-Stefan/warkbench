using System.ComponentModel;
using System.Runtime.CompilerServices;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Scenes;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

internal class World : IWorld
{
    // --- Properties ---
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LocalPath LocalPath { get; init; }
    public required int TileSize { get; init; } = 32;

    public required int ChunkResolution
    {
        get => _chunkResolution;
        set => Set(ref _chunkResolution, value);
    }
    
    public string Description
    {
        get => _description;
        set => Set(ref _description, value);
    }
    
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => Set(ref _createdAt, value);
    }
    
    public DateTime LastModifiedAt
    {
        get => _lastModifiedAt;
        private set => Set(ref _lastModifiedAt, value);
    }
    
    public bool IsDirty
    {
        get => _isDirty;
        set => Set(ref _isDirty, value);
    }
    
    public IScene? ActiveScene
    {
        get => _activeScene;
        set => Set(ref _activeScene, value);
    }
    
    // ---- Container ----
    
    public IEnumerable<object> Chunks => _chunks;
    public IEnumerable<IScene> Scenes => _scenes;
    public IEnumerable<IScene> LoadedScenes => _loadedScenes;
    
    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    // --- Fields ---

    private int _chunkResolution = 32;
    private string _description = string.Empty;
    private DateTime _createdAt = DateTime.Now;
    private DateTime _lastModifiedAt = DateTime.Now;
    private bool _isDirty;
    private IScene? _activeScene;
    
    private readonly List<object> _chunks = [];
    private readonly List<IScene> _scenes = [];
    private readonly List<IScene> _loadedScenes = [];
}