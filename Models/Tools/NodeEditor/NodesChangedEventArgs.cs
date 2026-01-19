using System;


namespace warkbench.Models;

public enum NodeChangeType
{
    New,
    Added,
    Removed,
    Cleared
}

/// <summary>
/// Provides detailed information about a change in the node collection
/// of a <see cref="NodeEditorModel"/>. Indicates the type of change
/// that occurred and optionally the affected node.
/// </summary>
public sealed class NodesChangedEventArgs : EventArgs
{
    /// <summary>
    /// Describes what kind of change occurred in the node collection.
    /// </summary>
    public required NodeChangeType Type { get; init; }

    /// <summary>
    /// The affected node, or <c>null</c> when no specific node applies (e.g. Clear).
    /// </summary>
    public NodeModel? Node { get; init; }
}