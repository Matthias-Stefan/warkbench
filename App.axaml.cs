using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Avalonia.Input;
using Nodify;
using Nodify.Compatibility;
using warkbench.Infrastructure;
using warkbench.Interop;
using warkbench.Models;
using warkbench.src.basis.core.Common;
using warkbench.ViewModels;
using warkbench.Views;
using UnixPath = warkbench.src.basis.core.Common.UnixPath;

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

        private void ConfigureServices(IServiceCollection services)
        {
            // NEW NEW NEW
            #if true
            var logger_NEW = new ConsoleLogger();
            var pathService_NEW = new warkbench.src.basis.core.Common.PathService(logger_NEW);
            var projectIoService_NEW = new warkbench.src.basis.core.Common.ProjectIoService(pathService_NEW, logger_NEW);
            var projectService_NEW = new warkbench.src.basis.core.Projects.ProjectService(projectIoService_NEW, pathService_NEW, logger_NEW);

            var projects = projectIoService_NEW.DiscoverProjects(pathService_NEW.ProjectPath, false);

            var enumerable = projects as string[] ?? projects.ToArray();
            if (enumerable.Length == 0)
            {
                logger_NEW.Info("Project file not found. Creating a new default project...");
                
                var defaultProject = projectService_NEW.CreateProject("warpunk_emberfall.wbproj");
                projectIoService_NEW.Save(defaultProject, warkbench.src.basis.core.Common.UnixPath.Combine(pathService_NEW.ProjectPath, defaultProject.Name));
            }
            else
            {
                projectService_NEW.LoadProject(enumerable[0]);    
            }
            #endif
            
            
            
            
            // Platform
            services.AddSingleton<IPlatformLibrary>(PlatformLibraryFactory.GetPlatformLibrary());
            var pathService = new warkbench.Infrastructure.PathService();
            services.AddSingleton(pathService);

            // Services
            services.AddSingleton<ISelectionService, SelectionService>();
            var ioService = new IOService(pathService);
            services.AddSingleton(ioService);

            // Editor/Browser data models (non-visual state holders)
            services.AddTransient<AssetEditorModel>();
            services.AddTransient<ContentBrowserModel>();
            services.AddTransient<DetailsModel>();
            services.AddTransient<NodeEditorModel>();

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
                    sp.GetRequiredService<ContentBrowserModel>(),
                    sp.GetRequiredService<warkbench.Infrastructure.PathService>(),
                    sp.GetRequiredService<ISelectionService>());
            });
            services.AddTransient<Func<DetailsViewModel>>(sp =>
            {
                return () => new DetailsViewModel(
                    sp.GetRequiredService<DetailsModel>(),
                    sp.GetRequiredService<ISelectionService>());
            });
            services.AddTransient<Func<AssetEditorViewModel>>(sp =>
            {
                return () => new AssetEditorViewModel(
                    sp.GetRequiredService<IProjectManager>(),
                    sp.GetRequiredService<AssetEditorModel>(),
                    sp.GetRequiredService<ISelectionService>());
            });
            services.AddTransient<Func<NodeEditorViewModel>>(sp =>
            {
                return () => new NodeEditorViewModel(
                    sp.GetRequiredService<IProjectManager>(),
                    sp.GetRequiredService<NodeEditorModel>(),
                    sp.GetRequiredService<ISelectionService>()
                    );
            });
        }

        private static void ConfigureNodify()
        {
            NodifyEditor.EnableDraggingContainersOptimizations = false;
            NodifyEditor.EnableCuttingLinePreview = true;
            
            EditorGestures.Mappings.Connection.Disconnect.Value = new AnyGesture(new MouseGesture(MouseAction.LeftClick, KeyModifiers.Alt), new MouseGesture(MouseAction.RightClick));
        }
    }
}