using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Avalonia.Input;
using Nodify;
using Nodify.Compatibility;
using warkbench.Infrastructure;
using warkbench.Interop;
using warkbench.Models;
using warkbench.ViewModels;
using warkbench.Views;

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
            // Platform
            services.AddSingleton<IPlatformLibrary>(PlatformLibraryFactory.GetPlatformLibrary());
            var pathService = new PathService();
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
            projectManager.Load(UnixPath.Combine(pathService.DataPath, "warpunk.emberfall.json"));
            
            // Dock layout factory: creates and connects editor components
            services.AddSingleton<DockFactory>();
            
            // Root-level ViewModel for the main application window
            services.AddSingleton<MainWindowViewModel>();
            
            // ViewModel factories with auto-resolved model + services
            services.AddTransient<Func<ContentBrowserViewModel>>(sp =>
            {
                return () => new ContentBrowserViewModel(
                    sp.GetRequiredService<ContentBrowserModel>(),
                    sp.GetRequiredService<PathService>(),
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