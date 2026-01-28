using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using warkbench.src.basis.core.Paths;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench.src.basis.core.Projects;

internal class Project : IProject
{
    public void AddWorld(LocalPath worldPath)
    {
        if (_worlds.Contains(worldPath))
            return;

        _worlds.Add(worldPath);
        
        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));

        ActiveWorldPath = worldPath;
    }

    public void RemoveWorld(LocalPath worldPath)
    {
        if (!_worlds.Remove(worldPath))
            return;

        if (Equals(ActiveWorldPath, worldPath))
            ActiveWorldPath = null;
        
        LastModifiedAt = DateTime.Now;
        IsDirty = true;

        OnPropertyChanged(nameof(Worlds));
    }
    
    // --- Properties ---
    
    public required Guid Id { get; init; }
    public required string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            NameChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public required LocalPath LocalPath { get; init; }

    public required string Version { get; init; }
    
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }
    
    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetProperty(ref _createdAt, value);
    }
    
    public DateTime LastModifiedAt
    {
        get => _lastModifiedAt;
        private set => SetProperty(ref _lastModifiedAt, value);
    }
    
    [JsonIgnore]
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            SetProperty(ref _isDirty, value);
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public IReadOnlyList<LocalPath> Worlds => _worlds;
    
    public LocalPath? ActiveWorldPath { get; set; }

    // ---- Events ----
    
    public event EventHandler? NameChanged;
    public event EventHandler? IsDirtyChanged;
    
    // ---- INotifyPropertyChanged ----
    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value!;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    // --- Fields ---

    private DateTime _createdAt = DateTime.Now;
    private DateTime _lastModifiedAt = DateTime.Now;
    private bool _isDirty;
    private string _description = string.Empty;
    private string _name = string.Empty;
    
    private readonly List<LocalPath> _worlds = [];
}