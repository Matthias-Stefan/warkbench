using System;
using warkbench.Models;


namespace warkbench.ViewModels;
public static class NodeFactory
{
    public static NodeViewModel CreateFromModel(NodeModel model)
    {
        return model switch
        {
            ClassNodeModel   m => new ClassNodeViewModel(m),
            FloatNodeModel   m => new FloatNodeViewModel(m),
            IntNodeModel     m => new IntNodeViewModel(m),
            StringNodeModel  m => new StringNodeViewModel(m),
            TextureNodeModel m => new TextureNodeViewModel(m),
            BoolNodeModel    m => new BoolNodeViewModel(m),
            RectNodeModel    m => new RectNodeViewModel(m),
            Vec2NodeModel    m => new Vec2NodeViewModel(m),
            
            _ => throw new NotSupportedException(
                $"Unsupported NodeModel type: {model.GetType().Name}")
        };
    }
}