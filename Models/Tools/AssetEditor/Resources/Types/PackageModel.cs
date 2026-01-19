using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace warkbench.Models;

public class PackageModel
{
    [JsonProperty]
    public required Guid Guid { get; set; } = Guid.Empty;
    
    [JsonProperty]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty]
    public required Guid BlueprintGuid { get; set; } = Guid.Empty;
    
    [JsonProperty]
    public HashSet<GraphModel> PackageItems { get; } = [];
}