using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Scenes;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Worlds;

internal class World : IWorld
{
    // --- Properties ---
    
    public required Guid Id { get; init; }

    public required string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            NameChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public required LocalPath LocalPath { get; init; }
    public required int TileSize { get; init; } = 32;

    public required int ChunkResolution
    {
        get => _chunkResolution;
        set => SetProperty(ref _chunkResolution, value);
    }
    
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }
    
    public DateTime LastModifiedAt
    {
        get => _lastModifiedAt;
        private set => SetProperty(ref _lastModifiedAt, value);
    }
    
    [JsonIgnore]
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            SetProperty(ref _isDirty, value);
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public IScene? ActiveScene
    {
        get => _activeScene;
        set => SetProperty(ref _activeScene, value);
    }
    
    // ---- Container ----
    
    public IEnumerable<object> Chunks => _chunks;
    public IEnumerable<IScene> Scenes => _scenes;
    public IEnumerable<IScene> LoadedScenes => _loadedScenes;
    
    // ---- Events ----
    
    public event EventHandler? NameChanged;
    public event EventHandler? IsDirtyChanged;
    
    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    // --- Fields ---

    private DateTime _createdAt = DateTime.Now;
    private DateTime _lastModifiedAt = DateTime.Now;
    private bool _isDirty;
    private string _description = string.Empty;
    private string _name = string.Empty;
    
    private int _chunkResolution = 32;
    private IScene? _activeScene;
    
    private readonly List<object> _chunks = [];
    private readonly List<IScene> _scenes = [];
    private readonly List<IScene> _loadedScenes = [];
}