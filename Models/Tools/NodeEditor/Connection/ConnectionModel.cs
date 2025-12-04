using Newtonsoft.Json;

namespace warkbench.Models;

public class ConnectionModel
{
    [JsonProperty]
    public required System.Guid Guid { get; set; } = System.Guid.Empty;
    
    [JsonProperty]
    public required ConnectorModel Source { get; set; }
    
    [JsonProperty]
    public required ConnectorModel Target { get; set; }
}