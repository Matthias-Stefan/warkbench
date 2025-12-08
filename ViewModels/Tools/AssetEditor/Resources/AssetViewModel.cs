using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;


namespace warkbench.ViewModels;
public abstract partial class AssetViewModel : ObservableObject, INameable
{
    protected abstract string GetName();
    protected abstract void SetName(string value);

    protected abstract string GetVirtualPath();
    protected abstract void SetVirtualPath(string value);
    
    public string Name
    {
        get => GetName();
        set
        {
            if (GetName() == value)
                return;

            SetName(value);
            OnPropertyChanged(nameof(Name));
        }
    }

    public string VirtualPath
    {
        get => GetVirtualPath();
        set
        {
            if (GetVirtualPath() == value)
                return;

            SetVirtualPath(value);
            OnPropertyChanged(nameof(VirtualPath));
        }
    }
}