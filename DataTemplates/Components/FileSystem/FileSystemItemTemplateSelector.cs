using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using warkbench.ViewModels;

namespace warkbench.DataTemplates;

public class FileSystemItemTemplateSelector : IDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new Dictionary<string, IDataTemplate>();

    public Control Build(object? param)
    {
        if (param is not FileSystemItemViewModel item)
        {
            throw new ArgumentNullException(nameof(param));
        }

        var key = item.ItemType.ToString();
        return AvailableTemplates[key].Build(param);
    }

    public bool Match(object? data)
    {
        if (data is not FileSystemItemViewModel item)
        {
            return false;
        }

        var key = item.ItemType.ToString();

        return !string.IsNullOrEmpty(key) && AvailableTemplates.ContainsKey(key);
    }
}