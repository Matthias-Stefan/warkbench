using System;
using Newtonsoft.Json;


namespace warkbench.Models;
public class Int64NodeModel : NodeModel
{
    public Int64NodeModel()
    {
        Title = "Int64";
    }
    
    public override NodeModel DeepClone()
    {
        var model = new Int64NodeModel
        {
            Guid = System.Guid.NewGuid(),
            Title = Title,
            Name = Name,
            Description = Description,
            Location = new Avalonia.Point(Location.X, Location.Y),
            NodeHeaderBrushType = NodeHeaderBrushType,
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
    
    [JsonProperty]
    public Int64 Value { get; set; } = 0;
}