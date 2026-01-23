using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));

        ActiveWorld ??= world;
    }

    public void RemoveWorld(IWorld world)
    {
        if (!_worlds.Remove(world))
            return;

        if (ReferenceEquals(ActiveWorld, world))
            ActiveWorld = _worlds.FirstOrDefault();

        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));
    }
    
    // --- Properties ---
    
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required LocalPath LocalPath { get; init; }
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
    
    public bool IsDirty
    {
        get => _isDirty;
        set => Set(ref _isDirty, value);
    }
    
    public IWorld? ActiveWorld
    {
        get => _activeWorld;
        set => Set(ref _activeWorld, value);
    }

    // ---- Container ----
    
    public IEnumerable<IWorld> Worlds => _worlds;
    public IEnumerable<object> Packages => _packages;
    public IEnumerable<object> Blueprints => _blueprints;
    public IEnumerable<object> Properties => _properties;
    
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
    
    private string _description = string.Empty;
    private DateTime _createdAt = DateTime.Now;
    private DateTime _lastModifiedAt = DateTime.Now;
    private bool _isDirty;
    private IWorld? _activeWorld;
    
    private readonly List<IWorld> _worlds = [];
    private readonly List<object> _packages = [];
    private readonly List<object> _blueprints = [];
    private readonly List<object> _properties = [];
}