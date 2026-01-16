using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.basis.core.Worlds;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

internal partial class Project : ObservableObject, IProject
{
    public void AddWorld(IWorld world)
    {
        throw new NotImplementedException();
    }

    public void RemoveWorld(IWorld world)
    {
        throw new NotImplementedException();
    }
    
    public required Guid Id
    {
        get => _id;
        init => SetProperty(ref _id, value);
    }

    public required string Name
    {
        get => _name;
        init => SetProperty(ref _name, value);
    }
    
    [ObservableProperty]
    private string _description = string.Empty;
    
    public required string LocalPath
    {
        get => _localPath;
        init => SetProperty(ref _localPath, value);
    }
    
    [ObservableProperty]
    private DateTime _createdAt = DateTime.Now;
    
    [ObservableProperty]
    private DateTime _lastModifiedAt = DateTime.Now;
    
    public required string Version
    {
        get => _version;
        init => SetProperty(ref _version, value);
    }

    [ObservableProperty] 
    private bool _isDirty = false;
    
    public IEnumerable<IWorld> Worlds { get; } = new List<World>();
    
    [ObservableProperty] 
    private IWorld? _activeWorld = null;
    
    public IEnumerable<object> Packages { get; } = new List<object>();
    
    public IEnumerable<object> Blueprints { get; } = new List<object>();
    
    public IEnumerable<object> Properties { get; } = new List<object>();
    
    private readonly Guid _id = Guid.NewGuid();
    private readonly string _name = string.Empty;
    private readonly string _localPath = string.Empty;
    private readonly string _version = string.Empty;
}