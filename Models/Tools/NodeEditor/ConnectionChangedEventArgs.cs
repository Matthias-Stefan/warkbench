using System;


namespace warkbench.Models;
public enum ConnectionChangeType
{
    New,
    Added,
    Removed,
    Cleared
}

/// <summary>
/// Provides detailed information about a change in the connection collection
/// of a <see cref="NodeEditorModel"/>. Indicates the type of change
/// that occurred and optionally the affected connection.
/// </summary>
public sealed class ConnectionsChangedEventArgs : EventArgs
{
    /// <summary>
    /// Describes what kind of change occurred in the connection collection.
    /// </summary>
    public required ConnectionChangeType Type { get; init; }

    /// <summary>
    /// The affected connection, or <c>null</c> when no specific connection applies (e.g. Clear).
    /// </summary>
    public ConnectionModel? Connection { get; init; }
}