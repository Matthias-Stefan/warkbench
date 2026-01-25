using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

internal class Project : IProject
{
    public void AddWorld(IWorld world)
    {
        if (_worlds.Contains(world))
            return;

        _worlds.Add(world);
        WorldPaths.Add(world.LocalPath);
        
        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));

        ActiveWorld = world;
        ActiveWorldPath = world.LocalPath;
    }

    public void RemoveWorld(IWorld world)
    {
        if (!_worlds.Remove(world) || !WorldPaths.Remove(world.LocalPath))
            return;

        if (ReferenceEquals(ActiveWorld, world))
        {
            ActiveWorld = null;
            ActiveWorldPath = null;
        }
        
        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));
    }
    
    // --- Properties ---
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }

    public required LocalPath LocalPath
    {
        get => _localPath;
        init
        {
            _localPath = value;
            ProjectPath = value.Parent();
            WorldsFolderPath = new LocalPath(UnixPath.Combine(ProjectPath.Value, IProject.WorldsFolderName));
            ScenesFolderPath = new LocalPath(UnixPath.Combine(ProjectPath.Value, IProject.ScenesFolderName));
            PackagesFolderPath = new LocalPath(UnixPath.Combine(ProjectPath.Value, IProject.PackagesFolderName));
            BlueprintsFolderPath = new LocalPath(UnixPath.Combine(ProjectPath.Value, IProject.BlueprintsFolderName));
            PropertiesFolderPath = new LocalPath(UnixPath.Combine(ProjectPath.Value, IProject.PropertiesFolderName));
        }
    }
    public required string Version { get; init; }
    
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
    
    [JsonIgnore]
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            Set(ref _isDirty, value);
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [JsonIgnore]
    public IWorld? ActiveWorld
    {
        get => _activeWorld;
        set => Set(ref _activeWorld, value);
    }

    public LocalPath? ActiveWorldPath { get; set; }

    // ---- Container ----
    [JsonIgnore] public IEnumerable<IWorld> Worlds => _worlds;
    [JsonIgnore] public IEnumerable<object> Packages => _packages;
    [JsonIgnore] public IEnumerable<object> Blueprints => _blueprints;
    [JsonIgnore] public IEnumerable<object> Properties => _properties;

    public List<LocalPath> WorldPaths { get; init; } = [];

    public List<LocalPath> PackagePaths { get; init; } = [];

    public List<LocalPath> BlueprintPaths { get; init; } = [];

    public List<LocalPath> PropertyPaths { get; init; } = [];
    
    [JsonIgnore] public LocalPath ProjectPath { get; private init; }
    [JsonIgnore] public LocalPath WorldsFolderPath { get; private init; }
    [JsonIgnore] public LocalPath ScenesFolderPath { get; private init; }
    [JsonIgnore] public LocalPath PackagesFolderPath { get; private init; }
    [JsonIgnore] public LocalPath BlueprintsFolderPath { get; private init; }
    [JsonIgnore] public LocalPath PropertiesFolderPath { get; private init; }

    // ---- Events ----
    
    public event EventHandler? IsDirtyChanged;
    
    // ---- INotifyPropertyChanged ----
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value!;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    // --- Fields ---

    private DateTime _createdAt = DateTime.Now;
    private DateTime _lastModifiedAt = DateTime.Now;
    private IWorld? _activeWorld;
    private LocalPath _localPath;
    private bool _isDirty;
    private string _description = string.Empty;
    
    private readonly List<IWorld> _worlds = [];
    private readonly List<object> _packages = [];
    private readonly List<object> _blueprints = [];
    private readonly List<object> _properties = [];
}