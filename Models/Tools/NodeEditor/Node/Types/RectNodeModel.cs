using Newtonsoft.Json;


namespace warkbench.Models;
public class RectNodeModel : NodeModel
{
    public RectNodeModel()
    {
        Title = "Rect";
    }
    
    public override NodeModel DeepClone()
    {
        var model = new RectNodeModel
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
    public int X { get; set; } = 0;
    
    [JsonProperty] 
    public int Y { get; set; } = 0;
    
    [JsonProperty] 
    public int Width { get; set; } = 0;
    
    [JsonProperty] 
    public int Height { get; set; } = 0;
}