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
    
    /// <summary>Gets the scope of the currently active selection.</summary>
    SelectionScope SelectedScope { get; }
    
    /// <summary>Gets the currently active (focused) selection.</summary>
    object? SelectedItem { get; }

    /// <summary>Gets the last project within the project scope, if any.</summary>
    IProject? LastSelectedProject { get; }

    /// <summary>Gets the last world within the world scope, if any.</summary>
    IWorld? LastSelectedWorld { get; }
}