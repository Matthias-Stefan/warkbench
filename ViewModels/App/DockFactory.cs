using CommunityToolkit.Mvvm.Input;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;
using Dock.Model.Mvvm.Core;
using System;
using warkbench.src.editors.workspace_explorer.ViewModels;


namespace warkbench.ViewModels;

public class DocumentViewModel : Document
{
}

public class DashboardViewModel : DockBase
{
}

public class HomeViewModel : RootDock
{
}

public class CustomDocumentDock : DocumentDock
{
    public CustomDocumentDock()
    {
        CreateDocument = new RelayCommand(CreateNewDocument);
    }

    private void CreateNewDocument()
    {
        if (!CanCreateDocument)
        {
            return;
        }

        var index = VisibleDockables?.Count + 1;
        var document = new DocumentViewModel { Id = $"Document{index}", Title = $"Document{index}" };

        Factory?.AddDockable(this, document);
        Factory?.SetActiveDockable(document);
        Factory?.SetFocusedDockable(this, document);
    }
}

public class DockFactory : Dock.Model.Mvvm.Factory
{
    private IRootDock? _rootDock;
    private IDocumentDock? _documentDock;

    private readonly Func<ContentBrowserViewModel> _createContentBrowserFunc;
    private readonly Func<DetailsViewModel> _createDetailsFunc;
    private readonly Func<WorkspaceExplorerViewModel> _createWorkspaceExplorerFunc;
    private readonly Func<NodeEditorViewModel> _createNodeEditorFunc;

    public override IRootDock CreateLayout()
    {
        // Content Browser
        // Details
        // Outliner
            
        var document1 = new WorldDocumentViewModel { Id = "Camera1", Title = "Camera1" };
        var document2 = new DocumentViewModel { Id = "Document2", Title = "Document2" };
        var document3 = new DocumentViewModel { Id = "Document3", Title = "Document3", CanClose = true };

        var contentBrowser = _createContentBrowserFunc();
        contentBrowser.Id = "ContentBrowser";
        contentBrowser.Title = "Content Browser";

        var details = _createDetailsFunc();
        details.Id = "Details";
        details.Title = "Details";

        var nodeEditor = _createNodeEditorFunc();
        nodeEditor.Id = "NodeEditor";
        nodeEditor.Title = "Node Editor";
            
        var workspaceExplorer = _createWorkspaceExplorerFunc();
        workspaceExplorer.Id = "WorkspaceExplorer";
        workspaceExplorer.Title = "Workspace Explorer";

        var contentBrowserDock = new ProportionalDock
        {
            Proportion = 0.56,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = contentBrowser,
                    VisibleDockables = CreateList<IDockable>(contentBrowser),
                    Alignment = Alignment.Left,
                    // CanDrop = false
                }
            ),
            // CanDrop = false
        };

        var detailsDock = new ProportionalDock
        {
            Proportion = 0.25,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = details,
                    VisibleDockables = CreateList<IDockable>(details),
                    Alignment = Alignment.Right,
                    // CanDrop = false
                }
            ),
            // CanDrop = false
        };
            
        var nodeEditorDock = new ProportionalDock
        {
            Proportion = 1 - contentBrowserDock.Proportion,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = nodeEditor,
                    VisibleDockables = CreateList<IDockable>(nodeEditor),
                    Alignment = Alignment.Right,
                    // CanDrop = false
                }
            ),
            // CanDrop = false
        };

        var workspaceExplorerDock = new ProportionalDock
        {
            Proportion = 0.25,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = workspaceExplorer,
                    VisibleDockables = CreateList<IDockable>(workspaceExplorer),
                    Alignment = Alignment.Left,
                    // CanDrop = false
                }
            ),
            // CanDrop = false
        };

        var documentDock = new CustomDocumentDock
        {
            IsCollapsable = false,
            ActiveDockable = document1,
            VisibleDockables = CreateList<IDockable>(document1, document2, document3),
            CanCreateDocument = true,
            // CanDrop = false,
            EnableWindowDrag = true,
        };

        var mainLayout = new ProportionalDock
        {
            Orientation = Orientation.Vertical,
            VisibleDockables = CreateList<IDockable>
            (
                new ProportionalDock()
                {
                    Orientation = Orientation.Horizontal,
                    VisibleDockables = CreateList<IDockable>
                    (
                        workspaceExplorerDock,
                        new ProportionalDockSplitter(),
                        documentDock,
                        new ProportionalDockSplitter(),
                        detailsDock
                    )
                },
                new ProportionalDockSplitter(),
                new ProportionalDock()
                {
                    Proportion = 0.42,
                    Orientation = Orientation.Horizontal,
                    VisibleDockables = CreateList<IDockable>
                    (
                        contentBrowserDock,
                        new ProportionalDockSplitter(),
                        nodeEditorDock
                    )
                }
            )
        };

        var dashboardView = new DashboardViewModel
        {
            Id = "Dashboard",
            Title = "Dashboard"
        };

        var homeView = new HomeViewModel
        {
            Id = "Home",
            Title = "Home",
            ActiveDockable = mainLayout,
            VisibleDockables = CreateList<IDockable>(mainLayout)
        };

        var rootDock = CreateRootDock();

        rootDock.IsCollapsable = false;
        rootDock.ActiveDockable = dashboardView;
        rootDock.DefaultDockable = homeView;
        rootDock.VisibleDockables = CreateList<IDockable>(dashboardView, homeView);

        _documentDock = documentDock;
        _rootDock = rootDock;

        return rootDock;
    }

    public DockFactory(
        Func<ContentBrowserViewModel> createContentBrowserFunc,
        Func<DetailsViewModel> createDetailsFunc,
        Func<WorkspaceExplorerViewModel> createWorkspaceExplorerFunc,
        Func<NodeEditorViewModel> createNodeEditorFunc)
    {
        _createContentBrowserFunc = createContentBrowserFunc;
        _createDetailsFunc = createDetailsFunc;
        _createWorkspaceExplorerFunc = createWorkspaceExplorerFunc;
        _createNodeEditorFunc = createNodeEditorFunc;
    }
}