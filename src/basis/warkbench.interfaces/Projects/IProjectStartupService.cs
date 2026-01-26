namespace warkbench.src.basis.interfaces.Projects;

/// <summary>Runs the project startup workflow, including optional restoration of the last opened project.</summary>
public interface IProjectStartupService
{
    /// <summary>Executes the project startup workflow.</summary>
    Task RunAsync();
}