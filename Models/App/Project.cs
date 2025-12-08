using Newtonsoft.Json;
using System.Collections.ObjectModel;
using warkbench.Infrastructure;
using warkbench.ViewModels;


namespace warkbench.Models;
public sealed class Project
{
    public Project(PathService pathService)
    {
        _pathService = pathService;
        Packages.CollectionChanged += (_, _) => IsDirty = true;
        PackageBlueprints.CollectionChanged += (_, _) => IsDirty = true;
    }
    
    [JsonProperty]
    public string Name { get; set; }

    [JsonProperty]
    public ObservableCollection<PackageModel> Packages { get; } = [];
    
    [JsonProperty]
    public ObservableCollection<GraphModel> PackageBlueprints { get; } = [];
    
    [JsonProperty]
    public ObservableCollection<GraphModel> Properties { get; } = [];

    [JsonIgnore] 
    public bool IsDirty { get; set; } = false;
    
    [JsonIgnore]
    public string Path => UnixPath.Combine(_pathService.DataPath, "warpunk.emberfall.json");
    
    private readonly PathService _pathService;
}