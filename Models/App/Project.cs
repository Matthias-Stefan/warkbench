using Newtonsoft.Json;
using System.Collections.ObjectModel;
using warkbench.Infrastructure;
using warkbench.src.basis.interfaces.Worlds;


namespace warkbench.Models;

public sealed class Project
{
    public Project(PathService pathService)
    {
        _pathService = pathService;
        Worlds.CollectionChanged += (_, _) => IsDirty = true;
        Packages.CollectionChanged += (_, _) => IsDirty = true;
        Blueprints.CollectionChanged += (_, _) => IsDirty = true;
    }
    
    [JsonProperty]
    public string Name { get; set; }

    [JsonProperty]
    public ObservableCollection<IWorld> Worlds { get; } = [];
    
    [JsonIgnore]
    public IWorld? ActiveWorld { get; set; }
    
    [JsonProperty]
    public ObservableCollection<PackageModel> Packages { get; } = [];
    
    [JsonProperty]
    public ObservableCollection<GraphModel> Blueprints { get; } = [];
    
    [JsonProperty]
    public ObservableCollection<GraphModel> Properties { get; } = [];
    
    [JsonIgnore] 
    public bool IsDirty { get; set; } = false;
    
    [JsonIgnore]
    public string Path => UnixPath.Combine(_pathService.DataPath, "warpunk.emberfall.json");
    
    private readonly PathService _pathService;
}