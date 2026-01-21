using Avalonia.Controls.Templates;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Dock.Model.Core;
using System.Linq;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace warkbench;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;

        // Ensure UI assemblies are loaded
        var baseDir = AppContext.BaseDirectory;

        foreach (var dll in Directory.EnumerateFiles(baseDir, "warkbench*.dll"))
        {
            try
            {
                var asmName = AssemblyName.GetAssemblyName(dll).Name;
                if (asmName is null)
                    continue;

                if (!asmName.Contains(".ui", StringComparison.OrdinalIgnoreCase))
                    continue;

                var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => string.Equals(a.GetName().Name, asmName, StringComparison.OrdinalIgnoreCase));

                if (!alreadyLoaded)
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(dll);
            }
            catch
            {
                // ignore invalid / native / load failures
            }
        }

        var viewTypeName = param.GetType().FullName!;

        if (viewTypeName.Contains(".editors.", StringComparison.Ordinal))
            viewTypeName = viewTypeName.Replace(".editors.", ".ui.", StringComparison.Ordinal);

        viewTypeName = viewTypeName
            .Replace(".ViewModels.", ".Views.", StringComparison.Ordinal)
            .Replace("ViewModel", "View", StringComparison.Ordinal);

        // Type.GetType() won't find it unless it's in mscorlib/current assembly or assembly-qualified.
        // Search all loaded assemblies.
        var type = AppDomain.CurrentDomain
            .GetAssemblies()
            .Select(a => a.GetType(viewTypeName, throwOnError: false, ignoreCase: false))
            .FirstOrDefault(t => t is not null);

        if (type is null)
            return new TextBlock { Text = "Not Found: " + viewTypeName };

        if (!typeof(Control).IsAssignableFrom(type))
            return new TextBlock { Text = "Not a Control: " + viewTypeName };

        return (Control)Activator.CreateInstance(type)!;
    }

    public bool Match(object? data) => data is ObservableObject || data is IDockable;
}