namespace warkbench.src.basis.interfaces.Common;

/// <summary>Indicates that an object has a renameable name.</summary>
public interface IRenameable
{
    /// <summary>Gets or sets the name.</summary>
    string Name { get; set; }

    /// <summary>Raised when <see cref="Name"/> changes.</summary>
    event EventHandler? NameChanged;
}
