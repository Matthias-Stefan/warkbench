using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.editors.core.ViewModel;

/// <summary>View model for the create project dialog.</summary>
public sealed partial class CreateProjectViewModel(IPathService pathService) : ObservableObject
{
    /// <summary>Confirms the dialog and returns the entered project data.</summary>
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        RequestClose?.Invoke(new CreateProjectInfo
        {
            ProjectName = ProjectName,
            ProjectPath = ProjectPath,
            OpenAfterCreation = OpenAfterCreation
        });
    }

    /// <summary>Cancels the dialog without creating a project.</summary>
    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke(null);

    private bool CanConfirm()
        => !string.IsNullOrWhiteSpace(ProjectName)
           && !string.IsNullOrWhiteSpace(ProjectPath);
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private string _projectName = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    private string _projectPath = pathService.ProjectPath.Value;

    [ObservableProperty]
    private bool _openAfterCreation = true;
    
    /// <summary>Raised when the dialog requests to be closed.</summary>
    public event Action<CreateProjectInfo?>? RequestClose;
}