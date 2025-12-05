# Warkbench

## Guide: Adding a New Node Type

This document describes the complete workflow for introducing a new node type into the Warkbench Node Editor.
Each node type requires coordinated updates to views, view models, models, styling, and factory logic.

1. Create the Required Files

Every node type consists of:

- View (Data Template)
- ViewModel
- Model

2. Add a Node Color

Node types use unique brushes for icons and UI styling.
To keep colors consistent across the editor, add a brush to:

Path: `NodeBrushes.cs`

3. Register the Node in the Factory

All node construction is handled through the central factory:

Path: `NodeFactory.cs`

4. Update the NodeEditorViewModel

The editor ViewModel orchestrates everything related to nodes.

Path: `NodeEditorViewModel.cs`

- 4.1 Add the node to the NodeTypeMap
``` C++
NodeTypeMap[NodeType.<Name>] = typeof(<Name>NodeViewModel);
```

- 4.2 Add command to create the node (toolbar/menu)
``` C++
[RelayCommand]
private Task OnAdd<Name>Node(TransformGroup transform)
{
// Logic for placing the node at a toolbar-defined position
}
```

- 4.3 Add command to create the node using mouse position (context menu)
``` C++
[RelayCommand]
private Task OnAdd<Name>NodeFromMouse(TransformGroup transform)
{
// Logic for placing the node at the pointer location
}
```

5. Optional Add-Ons

Create Icon


