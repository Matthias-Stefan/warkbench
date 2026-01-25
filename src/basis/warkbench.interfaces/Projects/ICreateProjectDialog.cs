namespace warkbench.src.basis.interfaces.Projects;

public interface ICreateProjectDialog
{
    Task<CreateProjectInfo?> ShowAsync();
}