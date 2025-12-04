using System;
using warkbench.Models;


namespace warkbench.ViewModels;
public static class NodeFactory
{
    public static NodeViewModel CreateFromModel(NodeModel model)
    {
        return model switch
        {
            ClassNodeModel  m => new ClassNodeViewModel(m),
            IntNodeModel    m => new IntNodeViewModel(m),
            FloatNodeModel  m => new FloatNodeViewModel(m),
            StringNodeModel m => new StringNodeViewModel(m),
            
            _ => throw new NotSupportedException(
                $"Unsupported NodeModel type: {model.GetType().Name}")
        };
    }
}