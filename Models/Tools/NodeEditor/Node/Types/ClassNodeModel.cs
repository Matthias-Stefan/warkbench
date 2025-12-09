namespace warkbench.Models;
public class ClassNodeModel : NodeModel
{
    public ClassNodeModel()
    {
        Title = "Class";
    }

    public override NodeModel DeepClone()
    {
        var model = new ClassNodeModel
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
}