using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nodify.Compatibility;
using Nodify;
using warkbench.Interop;
using warkbench.ViewModels;
using warkbench.Views;
using warkbench.src.basis.core.Common;
using warkbench.src.basis.core.Projects;
using warkbench.src.basis.core.Worlds;
using warkbench.src.basis.interfaces.Common;
using warkbench.src.basis.interfaces.Projects;
using warkbench.src.basis.interfaces.Worlds;

namespace warkbench
{
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
                
                InitializeProjectAsync();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void DisableAvaloniaDataAnnotationValidation()
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

        private async void InitializeProjectAsync()
        {
            if (_host is null) 
                return;

            var pathService = _host.Services.GetRequiredService<IPathService>();
            var projectIoService = _host.Services.GetRequiredService<IProjectIoService>();
            var projectService = _host.Services.GetRequiredService<IProjectService>();
            var logger = _host.Services.GetRequiredService<ILogger>();
            
            try 
            {
                var projects = projectIoService.DiscoverProjects(pathService.ProjectPath).ToList();
        
                if (projects.Count == 0)
                {
                    logger.Warn("[App] No projects found in project path.");
                    return;
                }

                var projectPath = projects.First();
                logger.Info($"[App] Auto-loading project: {projectPath}");

                await projectService.LoadProjectAsync(projectPath);
            }
            catch (Exception ex)
            {
                logger.Error($"[App] Critical error during startup initialization: {ex.Message}");
            }
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            // --- init core lib ---
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton<IPathService, PathService>();

            // project
            services.AddSingleton<IProjectIoService, ProjectIoService>();
            services.AddSingleton<IProjectService, ProjectService>();
            
            // world
            services.AddSingleton<IWorldIoService, WorldIoService>();
            services.AddSingleton<IWorldService, WorldService>();
            
            
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
            projectManager.Load(warkbench.Infrastructure.UnixPath.Combine(pathService.DataPath, "warpunk.emberfall.json"));
            
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
#endif
        }

        private static void ConfigureNodify()
        {
            NodifyEditor.EnableDraggingContainersOptimizations = false;
            NodifyEditor.EnableCuttingLinePreview = true;
            
            EditorGestures.Mappings.Connection.Disconnect.Value = new AnyGesture(new MouseGesture(MouseAction.LeftClick, KeyModifiers.Alt), new MouseGesture(MouseAction.RightClick));
        }
    }
}