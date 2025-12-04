using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace warkbench.Models;
public class GraphModel : IDeepCloneable<GraphModel>
{
    public GraphModel DeepClone()
    {
        Dictionary<NodeModel, NodeModel> dict = [];
        
        var result = new GraphModel
        {
            Guid = System.Guid.NewGuid(),
            Name = Name,
        };

        foreach (var node in Nodes)
        {
            var duplicatedModel = node.DeepClone();
            dict.Add(node, duplicatedModel);
            result.Nodes.Add(duplicatedModel);
        }

        foreach (var connection in Connections)
        {
            var sourceConnector = connection.Source;
            var targetConnector = connection.Target;

            var sourceNode = Nodes.FirstOrDefault(node => node.Outputs.Contains(sourceConnector));
            var targetNode = Nodes.FirstOrDefault(node => node.Inputs.Contains(targetConnector));

            if (sourceNode is null || targetNode is null)
            {
                continue;
            }

            var newSourceNode = dict[sourceNode];
            var newTargetNode = dict[targetNode];

            var newSourceConnector = newSourceNode.Outputs.FirstOrDefault(connector => !connector.IsConnected);
            var newTargetConnector = newTargetNode.Inputs.FirstOrDefault(connector => !connector.IsConnected);

            if (newSourceConnector is null || newTargetConnector is null)
            {
                continue;
            }

            result.Connections.Add(new ConnectionModel
            {
                Guid = System.Guid.NewGuid(),
                Source = newSourceConnector,
                Target = newTargetConnector
            });
            newSourceConnector.IsConnected = true;
            newTargetConnector.IsConnected = true;
        }
        
        return result;
    }
    
    [JsonProperty]
    public required System.Guid Guid { get; set; } = System.Guid.Empty;
    
    [JsonProperty]
    public required string Name { get; set; } = string.Empty;
    
    [JsonProperty]
    public HashSet<NodeModel> Nodes { get; set; } = [];
    
    [JsonProperty]
    public HashSet<ConnectionModel> Connections { get; set; } = [];
}
