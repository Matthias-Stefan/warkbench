using Newtonsoft.Json;

namespace warkbench.Models;

public class ConnectorModel
{
    [JsonProperty]
    public required System.Guid Guid { get; set; } = System.Guid.Empty;
    
    [JsonProperty]
    public string Title { get; set; } = string.Empty;

    [JsonProperty] 
    public Avalonia.Point Anchor { get; set; } = new(0, 0);

    [JsonProperty] 
    public bool IsConnected { get; set; } = false;
}