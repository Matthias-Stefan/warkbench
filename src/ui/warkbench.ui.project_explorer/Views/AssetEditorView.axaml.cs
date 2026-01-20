using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Interactivity;

// ReSharper disable once CheckNamespace
namespace warkbench.src.ui.project_explorer.Views;

public partial class AssetEditorView : UserControl
{
    public AssetEditorView()
    {
        InitializeComponent();

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }
    
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (IsFileViewModelOfType(e, ".obj", out _) || IsFileViewModelOfType(e, ".png", out _))
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
            return;
        }

        e.DragEffects = DragDropEffects.Move;
        e.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (!IsFileViewModelOfType(e, ".png", out var file))
        {
            return;    
        }

        if (DataContext is AssetEditorViewModel vm)
        {
            // TODO:
            //_ = vm.OnDropPng(file);
        }
    }

    private bool IsFileViewModelOfType(DragEventArgs e, string extension, [NotNullWhen(true)] out FileViewModel? outFile)
    {
        var dataFormat = typeof(FileViewModel).FullName;
        if (dataFormat != null && e.Data.Contains(dataFormat))
        {
            var fullName = typeof(FileViewModel).FullName;
            if (fullName != null &&
                e.Data.Get(fullName) is FileViewModel file &&
                string.Equals(System.IO.Path.GetExtension(file.FullPath), extension))
            {
                outFile = file;
                return true;
            }
        }

        outFile = null;
        return false;
    }
    
    private void MenuItem_AddPackage(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not AssetEditorViewModel assetEditorViewModel || sender is not MenuItem mi)
        {
            return;
        }

        var tag = mi.Tag;
        if (tag is not BlueprintViewModel packageBlueprintViewModel)
        {
            return;    
        }
        
        var packageModel = new PackageModel
        {
            Guid = System.Guid.NewGuid(),
            Name = packageBlueprintViewModel.Name,
            BlueprintGuid = packageBlueprintViewModel.Model.Guid
        };
        assetEditorViewModel.AddPackage(new PackageViewModel(packageModel, packageBlueprintViewModel.Model));
    }
}