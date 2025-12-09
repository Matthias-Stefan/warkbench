using Newtonsoft.Json;
using System.Collections.Generic;
using warkbench.Brushes;
using warkbench.ViewModels;


namespace warkbench.Models;
public abstract class NodeModel : IDeepCloneable<NodeModel>
{
    public abstract NodeModel DeepClone();
    
    [JsonProperty]
    public required System.Guid Guid { get; set; } = System.Guid.Empty;
    
    [JsonProperty] 
    public string Title { get; protected set; } = string.Empty;

    [JsonProperty]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty]
    public string Description { get; set; } = string.Empty;

    [JsonProperty] 
    public Avalonia.Point Location { get; set; } = new(0, 0);

    [JsonProperty] 
    public required NodeHeaderBrushType NodeHeaderBrushType { get; set; } = NodeHeaderBrushType.None;
    
    [JsonProperty]
    public HashSet<ConnectorModel> Inputs { get; set; } = [];
    
    [JsonProperty]
    public HashSet<ConnectorModel> Outputs { get; set; } = [];
}