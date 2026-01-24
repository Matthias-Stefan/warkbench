namespace warkbench.src.editors.core.Projects;

/// <summary>Contains user-defined data required to create a new project.</summary>
public sealed class CreateProjectInfo
{
    /// <summary>The display name of the project.</summary>
    public required string ProjectName { get; init; }

    /// <summary>The target directory where the project will be created.</summary>
    public required string ProjectPath { get; init; }

    /// <summary>Indicates whether the project should be opened after creation.</summary>
    public bool OpenAfterCreation { get; init; } = true;
}