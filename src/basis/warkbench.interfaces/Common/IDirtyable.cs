namespace warkbench.src.basis.interfaces.Common;

/// <summary>Indicates that an object can have unsaved changes.</summary>
public interface IDirtyable
{
    /// <summary>Gets whether the object has unsaved changes.</summary>
    bool IsDirty { get; set; }

    /// <summary>Raised when <see cref="IsDirty"/> changes.</summary>
    event EventHandler? IsDirtyChanged;
}