using Avalonia.Controls;
using Avalonia.Controls.Templates;


namespace warkbench.src.ui.workspace_explorer.Templates;

public class WorkspaceContextToolbarTemplateSelector : IDataTemplate
{
    public Control Build(object? data)
    {
        return data switch
        {
            //PackageViewModel => new PackageToolbarView(),
            _ => new Panel()
        };
    }

    public bool Match(object? data)
    {
        return true;
    }
}