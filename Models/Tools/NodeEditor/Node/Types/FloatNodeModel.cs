using Newtonsoft.Json;


namespace warkbench.Models;
public class FloatNodeModel : NodeModel
{
    public FloatNodeModel()
    {
        Title = "Float";
    }
    
    public override NodeModel DeepClone()
    {
        var model = new FloatNodeModel
        {
            Guid = System.Guid.NewGuid(),
            Title = Title,
            Name = Name,
            Description = Description,
            Location = new Avalonia.Point(Location.X, Location.Y),
            IsBlueprint = IsBlueprint,
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
    public float Value { get; set; } = 0.0f;
}