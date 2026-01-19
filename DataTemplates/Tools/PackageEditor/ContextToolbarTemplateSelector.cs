using Avalonia.Controls;
using Avalonia.Controls.Templates;
using warkbench.ViewModels;
using warkbench.Views;


namespace warkbench.DataTemplates;

public class ContextToolbarTemplateSelector : IDataTemplate
{
    public Control Build(object? data)
    {
        return data switch
        {
            PackageViewModel => new PackageToolbarView(),
            _ => new Panel()
        };
    }

    public bool Match(object? data)
    {
        return true;
    }
}