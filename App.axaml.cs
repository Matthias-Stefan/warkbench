using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nodify.Compatibility;
using Nodify;
using warkbench.Infrastructure;
using warkbench.Interop;
using warkbench.src.basis.core.App;
using warkbench.ViewModels;
using warkbench.Views;

using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Io;
using warkbench.src.basis.core.Projects;
using warkbench.src.basis.core.Worlds;
using warkbench.src.basis.interfaces.App;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Io;
using warkbench.src.basis.interfaces.Paths;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;
using warkbench.src.editors.workspace_explorer.ViewModels;
using warkbench.src.ui.core.Projects;

namespace warkbench;

public partial class App : Application
{
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureNodify();
            
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            var mainWindowViewModel = _host?.Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
              
            Avalonia.Threading.Dispatcher.UIThread.Post(async void () =>
            {
                try
                {
                    await InitializeProjectAsync();
                }
                catch (Exception ex)
                {
                    // TODO:
                }
            });
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }

    private async Task InitializeProjectAsync()
    {
        if (_host is null) 
            return;
            
        var logger = _host.Services.GetRequiredService<ILogger>();
        var appStateService = _host.Services.GetRequiredService<IAppStateService>();
        var projectService  = _host.Services.GetRequiredService<IProjectService>();
        var session = _host.Services.GetRequiredService<IProjectSession>();    
        
        try
        {
            // 1) Load persisted app state
            appStateService.Load();

            // 2) Try auto-open last project
            if (appStateService.State.LastProjectPath is not { } lastProjectPath)
                return;
            
            var project = await projectService.LoadProjectAsync(lastProjectPath);
            logger.Info<App>($"Auto-loading last project: {lastProjectPath.Value}");
            
            await session.SwitchToAsync(project);
        }
        catch (Exception e)
        {
            logger.Warn<App>("Stored application state references a project that no longer exists. " +
                             "Continuing startup without restoring the previous project.");
            appStateService.State.LastProjectPath = null;
            appStateService.Save();
        }
    }
        
    private void ConfigureServices(IServiceCollection services)
    {
        // --- init warkbench.core lib ---
        services.AddSingleton<ILogger, ConsoleLogger>();
        // TODO: remove full qualified!
        services.AddSingleton<IPathService, warkbench.src.basis.core.Paths.PathService>();
        
        // app
        services.AddSingleton<IAppStateIoService, AppStateIoService>();
        services.AddSingleton<IAppStateService, AppStateService>();
        
        // project
        services.AddSingleton<IProjectIoService, ProjectIoService>();
        services.AddSingleton<IProjectService, ProjectService>();
        services.AddSingleton<ISelectionService<IProject>, SelectionService<IProject>>();
        
        // world
        services.AddSingleton<IWorldIoService, WorldIoService>();
        services.AddSingleton<IWorldService, WorldService>();
        services.AddSingleton<ISelectionService<IWorld>, SelectionService<IWorld>>();

        // session
        services.AddSingleton<IProjectSession, ProjectSession>();
        
        // --- init warkbench.editors libs ---
        services.AddSingleton<WorkspaceExplorerViewModel>();
        
        // --- init warkbench.ui libs ---
        services.AddSingleton<ICreateProjectDialog, CreateProjectDialog>();
        
#if true
        // Platform
        services.AddSingleton<IPlatformLibrary>(PlatformLibraryFactory.GetPlatformLibrary());
        var pathService = new warkbench.Infrastructure.PathService();
        services.AddSingleton(pathService);

        // Services
        services.AddSingleton<Infrastructure.ISelectionService, Infrastructure.SelectionService>();
        var ioService = new Infrastructure.IOService(pathService);
        services.AddSingleton(ioService);

        // Editor/Browser data models (non-visual state holders)
        services.AddTransient<Models.AssetEditorModel>();
        services.AddTransient<Models.ContentBrowserModel>();
        services.AddTransient<Models.DetailsModel>();
        services.AddTransient<Models.NodeEditorModel>();

        // File loader
        services.AddSingleton<ObjLoader>();
        services.AddSingleton<ImageLoader>();
            
        // Project Manager
        var projectManager = new ProjectManager(ioService, pathService);
        services.AddSingleton<IProjectManager>(projectManager);
        //projectManager.Load(warkbench.Infrastructure.UnixPath.Combine(pathService.DataPath, "warpunk.emberfall.json"));
            
        // Dock layout factory: creates and connects editor components
        services.AddSingleton<DockFactory>();
            
        // Root-level ViewModel for the main application window
        services.AddSingleton<MainWindowViewModel>();
            
        // ViewModel factories with auto-resolved model + services
        services.AddTransient<Func<ContentBrowserViewModel>>(sp =>
        {
            return () => new ContentBrowserViewModel(
                sp.GetRequiredService<Models.ContentBrowserModel>(),
                sp.GetRequiredService<warkbench.Infrastructure.PathService>(),
                sp.GetRequiredService<Infrastructure.ISelectionService>());
        });
        services.AddTransient<Func<DetailsViewModel>>(sp =>
        {
            return () => new DetailsViewModel(
                sp.GetRequiredService<Models.DetailsModel>(),
                sp.GetRequiredService<Infrastructure.ISelectionService>());
        });
        services.AddTransient<Func<AssetEditorViewModel>>(sp =>
        {
            return () => new AssetEditorViewModel(
                sp.GetRequiredService<IProjectManager>(),
                sp.GetRequiredService<Models.AssetEditorModel>(),
                sp.GetRequiredService<Infrastructure.ISelectionService>(),
                // NEW
                sp.GetRequiredService<IProjectService>(),
                sp.GetRequiredService<IWorldService>());
        });
        services.AddTransient<Func<NodeEditorViewModel>>(sp =>
        {
            return () => new NodeEditorViewModel(
                sp.GetRequiredService<IProjectManager>(),
                sp.GetRequiredService<Models.NodeEditorModel>(),
                sp.GetRequiredService<Infrastructure.ISelectionService>()
            );
        });
        services.AddTransient<Func<WorkspaceExplorerViewModel>>(sp =>
        {
            return () => new WorkspaceExplorerViewModel(
                sp.GetRequiredService<IProjectSession>(),
                sp.GetRequiredService<ISelectionService<IProject>>(),
                sp.GetRequiredService<ISelectionService<IWorld>>());
        });
#endif
    }

    private static void ConfigureNodify()
    {
        NodifyEditor.EnableDraggingContainersOptimizations = false;
        NodifyEditor.EnableCuttingLinePreview = true;
            
        EditorGestures.Mappings.Connection.Disconnect.Value = new AnyGesture(new MouseGesture(MouseAction.LeftClick, KeyModifiers.Alt), new MouseGesture(MouseAction.RightClick));
    }
}