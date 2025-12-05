using Newtonsoft.Json;


namespace warkbench.Models;
public class Vec2NodeModel : NodeModel
{
    public Vec2NodeModel()
    {
        Title = "Vector 2D";
    }
    
    public override NodeModel DeepClone()
    {
        var model = new Vec2NodeModel
        {
            Guid = System.Guid.NewGuid(),
            Title = Title,
            Name = Name,
            Description = Description,
            Location = new Avalonia.Point(Location.X, Location.Y),
            IsBlueprint = IsBlueprint,
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
    public double X { get; set; } = 0.0;

    [JsonProperty]
    public double Y { get; set; } = 0.0;
}