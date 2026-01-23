using System.ComponentModel;
using System.Runtime.CompilerServices;
using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.core.App;

internal sealed class AppState : IAppState
{
    /// <inheritdoc />
    public LocalPath? LastProjectPath
    {
        get => _lastProjectPath;
        set => Set(ref _lastProjectPath, value);
    }
    
    // ---- INotifyPropertyChanged ----

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value!;
        OnPropertyChanged(propertyName);
        return true;
    }

    // --- Fields ---

    private LocalPath? _lastProjectPath;
}