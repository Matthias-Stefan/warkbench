using Newtonsoft.Json;


namespace warkbench.Models;

public class PropertyNodeModel : NodeModel
{
    public PropertyNodeModel()
    {
        Title = "Property";
    }
    
    public override NodeModel DeepClone()
    {
        var internalGraph = InternalGraph?.DeepClone() ?? null;
        if (internalGraph is not null)
        {
            foreach (var internalGraphNode in internalGraph.Nodes)
            {
                internalGraphNode.NodeHeaderBrushType = NodeHeaderBrushType;
            }
        }

        var model = new PropertyNodeModel()
        {
            Guid = System.Guid.NewGuid(),
            Title = Title,
            Name = Name,
            Description = Description,
            Location = new Avalonia.Point(Location.X, Location.Y),
            NodeHeaderBrushType = NodeHeaderBrushType,
            InternalGraph = InternalGraph?.DeepClone() ?? null,
        };

        foreach (var connector in Outputs)
        {
            model.Outputs.Add(new ConnectorModel
            {
                Guid = System.Guid.NewGuid(),
                Title = connector.Title,
                Anchor = new Avalonia.Point(connector.Anchor.X, connector.Anchor.Y),
                IsConnected =  false,
            });
        }

        return model;
    }
}