using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.editors.core.Worlds;

public sealed partial class CreateWorldViewModel(IPathService pathService, IProjectSession projectSession) : ObservableObject
{
    /// <summary>Confirms the dialog and returns the entered world data.</summary>
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        RequestClose?.Invoke(new CreateWorldInfo
        {
            WorldName = WorldName,
            WorldPath = WorldPath,
            TileSize = TileSize,
            ChunkSize = ChunkSize
        });
    }
    
    /// <summary>Cancels the dialog without creating a world.</summary>
    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke(null);

    private bool CanConfirm()
        => !string.IsNullOrWhiteSpace(WorldName)
        && !string.IsNullOrWhiteSpace(WorldPath)
        && TileSize > 0
        && ChunkSize > 0;

    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [ObservableProperty]
    private string _worldName = string.Empty;

    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))] 
    [ObservableProperty]
    private string _worldPath = string.Empty;

    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))] 
    [ObservableProperty]
    private int _tileSize = 32;
    
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))] 
    [ObservableProperty]
    private int _chunkSize = 32;
    
    
    /// <summary>Raised when the dialog requests to be closed.</summary>
    public event Action<CreateWorldInfo?>? RequestClose;
}