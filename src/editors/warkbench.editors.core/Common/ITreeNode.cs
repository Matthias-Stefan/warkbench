using System.Collections.ObjectModel;
using System.ComponentModel;
using warkbench.src.editors.core.Models;

// ReSharper disable once CheckNamespace
namespace warkbench.src.editors.core.ViewModel;

/// <summary>
/// Defines the contract for hierarchical data representation within editor tree structures.
/// Supports parent-child navigation, state management (selection/expansion), and recursive manipulation.
/// </summary>
public interface ITreeNode : INotifyPropertyChanged
{
    /// <summary> Adds a node to the children collection and sets its parent reference. </summary>
    void AddChild(ITreeNode node);
    
    /// <summary> Removes a specific node from children (recursive search possible). </summary>
    bool RemoveChild(ITreeNode node);
    
    /// <summary> Detaches this node from its current parent. </summary>
    void RemoveFromParent();
    
    /// <summary> Sets IsExpanded to true for this node and all its descendants. </summary>
    void ExpandAll();
    
    /// <summary> Sets IsExpanded to false for this node and all its descendants. </summary>
    void CollapseAll();
    
    /// <summary> Returns the full hierarchy path (e.g., "Project/Worlds/Level1"). </summary>
    string GetFullPath(string separator = "/");
    
    /// <summary>Gets the base name of the node without UI decorations.</summary>
    string Name { get; }

    /// <summary> Gets the UI display name of the node, including state-based decorations </summary>
    string DisplayName { get; }
    
    /// <summary> Reference to the parent node. Null for root elements. </summary>
    ITreeNode? Parent { get; }
    
    /// <summary> List of nested child nodes. </summary>
    ReadOnlyObservableCollection<ITreeNode> Children { get; }
    
    /// <summary> The underlying warkbench.core data object (e.g., World, Project, Asset). </summary>
    object? Data { get; }
    
    /// <summary> Gets or sets whether the node is currently expanded in the UI. </summary>
    bool IsExpanded { get; set; }
    
    /// <summary> Gets or sets whether the node is currently selected. </summary>
    bool IsSelected { get; set; }

    /// <summary>Gets or sets whether the object has unsaved changes.</summary>
    bool IsDirty { get; }

    /// <summary> Indicates if the node is at the top of the hierarchy. </summary>
    bool IsRoot => Parent is null;
    
    /// <summary>Gets the current loading state of the item.</summary>
    LoadState? LoadState { get; }
    
    string LoadStateText { get; }
}