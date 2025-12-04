using System;
using CommunityToolkit.Mvvm.Input;
using Dock.Model.Mvvm.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using warkbench.Infrastructure;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class ContentBrowserViewModel : Tool
{
    private readonly ISelectionService _selectionService;

    private FolderViewModel? _selectedFavoritesItem;
    private FolderViewModel? _selectedExplorerItem;

    public ContentBrowserModel Model { get; }

    public ObservableCollection<FolderViewModel> Favorites { get; }
    public FolderViewModel? SelectedFavoritesItem
    {
        get => _selectedFavoritesItem;
        set
        {
            _selectedFavoritesItem = value;
            if (_selectedFavoritesItem is not null)
            {
                Open(_selectedFavoritesItem);
            }
        }
    }

    public ObservableCollection<FolderViewModel> Explorer { get; }
    public FolderViewModel? SelectedExplorerItem
    {
        get => _selectedExplorerItem;
        set
        {
            _selectedExplorerItem = value;
            if (_selectedExplorerItem is not null)
            {
                Open(_selectedExplorerItem);    
            }
        }
    }

    public ObservableCollection<FileSystemItemViewModel> CurrentLevel { get; }
    public ObservableCollection<BreadcrumbViewModel> BreadcrumbBar { get; }
    private FolderViewModel Home { get; }

    public void Open(FolderViewModel folder)
    {
        CurrentLevel.Clear();

        foreach (var file in folder.Files)
        {
            CurrentLevel.Add(file);
        }

        foreach (var subFolder in folder.SubFolders)
        {
            CurrentLevel.Add(subFolder);
        }

        _selectedExplorerItem = folder;
        OnPropertyChanged(nameof(SelectedExplorerItem));
        GetBreadcrumbPath(folder);
    }

    private static async Task CreateHierarchyAsync(FolderViewModel folder)
    {
        var (files, dirs) = await Task.Run(() =>
        {
            var files = Directory.GetFiles(folder.FullPath);
            var dirs = Directory.GetDirectories(folder.FullPath);
            return (files, dirs);
        });

        foreach (var path in files)
        {
            var extension = System.IO.Path.GetExtension(path);
            var model = new FileModel(path);
            var file = extension switch
            {
                ".cpp" => new CppFileViewModel(model) { Parent = folder },
                ".hpp" => new HppFileViewModel(model) { Parent = folder },
                ".png" => new PngFileViewModel(model) { Parent = folder },
                _ => new FileViewModel(model) { Parent = folder }
            };

            folder.Files.Add(file);
        }

        foreach (var path in dirs)
        {
            var subFolder = new FolderViewModel(new FolderModel(path)) { Parent = folder };
            folder.SubFolders.Add(subFolder);

            await CreateHierarchyAsync(subFolder);
        }
    }

    private void GetBreadcrumbPath(FolderViewModel? folder)
    {
        BreadcrumbBar.Clear();
        if (folder is not null && folder == Home)
        {
            return;
        }

        var breadcrumbPath = new List<BreadcrumbViewModel>();
        while (folder is { Parent: not null })
        {
            breadcrumbPath.Add(new BreadcrumbViewModel(folder));
            folder = folder.Parent;
        }
        breadcrumbPath.Reverse();
        foreach (var breadcrumb in breadcrumbPath)
        {
            BreadcrumbBar.Add(breadcrumb);
        }
    }

    public async void AddToHome(FolderViewModel folder)
    {
        try
        {
            folder.IsSelectable = false;
            Explorer.Add(folder);
        
            await CreateHierarchyAsync(folder);

            folder.IsSelectable = true;
            folder.Parent = Home;
            Home.SubFolders.Add(folder);
        
            if (BreadcrumbBar.Count == 0)
            {
                CurrentLevel.Add(folder);
            }
        }
        catch (Exception e)
        {
            throw; // TODO handle exception
        }
    }

    private void MarkAsFavorite(FolderViewModel folder)
    { 
        Favorites.Add(folder);
    }

    private void UnmarkAsFavorite(FolderViewModel folder)
    { 
        Favorites.Remove(new FolderViewModel(folder.Model));
    }

    [RelayCommand]
    private Task OnHomeButtonPressed()
    {
        Open(Home);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnBreadcrumbPressed(FolderViewModel folder)
    {
        Open(folder);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnMarkAsFavorite(FolderViewModel folder)
    {
        MarkAsFavorite(folder);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnUnmarkAsFavorite(FolderViewModel folder)
    {
        UnmarkAsFavorite(folder);
        return Task.CompletedTask;
    }

    public ContentBrowserViewModel(
        ContentBrowserModel model,
        PathService pathService,
        ISelectionService selectionService)
    {
        Model = model;
        _selectionService = selectionService;

        Home = new FolderViewModel(new FolderModel())
        {
            Parent = null
        };

        Favorites = [];
        Explorer = [];
        CurrentLevel = [];
        BreadcrumbBar = [];

        foreach (var path in new string[] { pathService.DataPath, pathService.AssetsPath })
        {
            var folder = new FolderViewModel(new FolderModel(path));
            AddToHome(folder);
        }
    }
}
