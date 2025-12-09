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
        var model = new PropertyNodeModel
        {
            Guid = System.Guid.NewGuid(),
            Title = Title,
            Name = Name,
            Description = Description,
            Location = new Avalonia.Point(Location.X, Location.Y),
            NodeHeaderBrushType = NodeHeaderBrushType,
        };

        foreach (var connector in Inputs)
        {
            model.Inputs.Add(new ConnectorModel
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
    public object Value { get; set; } = new();
}