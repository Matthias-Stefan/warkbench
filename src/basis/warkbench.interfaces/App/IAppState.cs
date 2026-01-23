using System.ComponentModel;
using warkbench.src.basis.interfaces.Paths;

namespace warkbench.src.basis.interfaces.App;

/// <summary>Represents the persistent, user-specific application runtime state.</summary>
public interface IAppState : INotifyPropertyChanged
{
    /// <summary>Gets or sets the path of the last opened project file.</summary>
    LocalPath? LastProjectPath { get; set; }
}