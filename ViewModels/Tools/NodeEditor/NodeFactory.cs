using System;
using warkbench.Models;


namespace warkbench.ViewModels;
public static class NodeFactory
{
    public static NodeViewModel CreateFromModel(NodeModel model)
    {
        return model switch
        {
            BoolNodeModel     m => new BoolNodeViewModel(m),
            ClassNodeModel    m => new ClassNodeViewModel(m),
            FloatNodeModel    m => new FloatNodeViewModel(m),
            Int32NodeModel    m => new Int32NodeViewModel(m),
            Int64NodeModel    m => new Int64NodeViewModel(m),
            PropertyNodeModel m => new PropertyNodeViewModel(m),
            RectNodeModel     m => new RectNodeViewModel(m),
            StringNodeModel   m => new StringNodeViewModel(m),
            TextureNodeModel  m => new TextureNodeViewModel(m),
            Vec2NodeModel     m => new Vec2NodeViewModel(m),
            
            _ => throw new NotSupportedException(
                $"Unsupported NodeModel type: {model.GetType().Name}")
        };
    }
}