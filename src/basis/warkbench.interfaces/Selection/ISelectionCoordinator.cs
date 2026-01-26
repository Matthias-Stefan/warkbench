using System.ComponentModel;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.interfaces.Selection;

/// <summary>
/// Central coordinator that manages active UI selection and contextual selections across the workspace.
/// </summary>
public interface ISelectionCoordinator : INotifyPropertyChanged
{
    /// <summary>Subscribes to selection changes for the specified scopes.</summary>
    ISelectionSubscription Subscribe(params SelectionScope[] scopes);
    
    /// <summary>Activates a project as the current selection.</summary>
    void SelectProject(IProject project);

    /// <summary>Activates a world as the current selection and sets its owning project as context.</summary>
    void SelectWorld(IWorld world);
    
    /// <summary>Gets the currently active (focused) selection.</summary>
    object? ActiveSelection { get; }

    /// <summary>Gets the scope of the currently active selection.</summary>
    SelectionScope ActiveScope { get; }

    /// <summary>Gets the current project context.</summary>
    IProject? CurrentProject { get; }

    /// <summary>Gets the current world context, if any.</summary>
    IWorld? CurrentWorld { get; }
}