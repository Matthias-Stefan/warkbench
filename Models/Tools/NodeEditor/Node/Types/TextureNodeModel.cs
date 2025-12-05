using Newtonsoft.Json;


namespace warkbench.Models;
public class TextureNodeModel : NodeModel
{
    public TextureNodeModel()
    {
        Title = "Texture";
    }

    public override NodeModel DeepClone()
    {
        var model = new TextureNodeModel
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
    public string Value { get; set; } = "";
}