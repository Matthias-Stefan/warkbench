using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.src.basis.interfaces.Scenes;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.core.Worlds;

internal partial class World : ObservableObject, IWorld
{
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
    
    public IEnumerable<object> Chunks { get; }
    
    public IEnumerable<IScene> Scenes { get; }
    
    public IScene? ActiveScene { get; set; }
    
    public IEnumerable<IScene> LoadedScenes { get; }
    
    private readonly Guid _id = Guid.NewGuid();
    private readonly string _name = string.Empty;
    private readonly string _localPath = string.Empty;
}