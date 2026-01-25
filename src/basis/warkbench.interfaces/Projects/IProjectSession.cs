using System.ComponentModel;

namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Coordinates project switching and workspace state transitions.</summary>
public interface IProjectSession
{
    /// <summary>Switches the workspace to the specified project.</summary>
    void SwitchTo(IProject? project);

    /// <summary>Asynchronously switches the workspace to the specified project.</summary>
    Task SwitchToAsync(IProject? project);
}