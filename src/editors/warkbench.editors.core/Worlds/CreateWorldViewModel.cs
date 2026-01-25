using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;

namespace warkbench.src.editors.core.Worlds;

public sealed partial class CreateWorldViewModel() : ObservableObject
{
    /// <summary>Confirms the dialog and returns the entered world data.</summary>
    [RelayCommand(CanExecute = nameof(CanConfirm))]
    private void Confirm()
    {
        RequestClose?.Invoke(new CreateWorldInfo
        {
            WorldName = WorldName,
            TileSize = TileSize,
            ChunkResolution = ChunkResolution
        });
    }
    
    /// <summary>Cancels the dialog without creating a world.</summary>
    [RelayCommand]
    private void Cancel() => RequestClose?.Invoke(null);

    private bool CanConfirm()
        => !string.IsNullOrWhiteSpace(WorldName)
        && TileSize > 0
        && ChunkResolution > 0;

    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))]
    [ObservableProperty]
    private string _worldName = string.Empty;

    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))] 
    [ObservableProperty]
    private int _tileSize = 32;
    
    [NotifyCanExecuteChangedFor(nameof(ConfirmCommand))] 
    [ObservableProperty]
    private int _chunkResolution = 32;
    
    /// <summary>Raised when the dialog requests to be closed.</summary>
    public event Action<CreateWorldInfo?>? RequestClose;
}