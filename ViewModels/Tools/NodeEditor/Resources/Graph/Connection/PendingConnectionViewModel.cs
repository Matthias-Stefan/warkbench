using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace warkbench.ViewModels;

public partial class PendingConnectionViewModel : ObservableObject
{
    public PendingConnectionViewModel(NodeEditorViewModel nodeEditorViewModel)
    {
        _nodeEditorViewModel = nodeEditorViewModel;
    }

    [RelayCommand]
    private Task OnStart(ConnectorViewModel source)
    {
        _source = source;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task OnFinish(ConnectorViewModel? target)
    {
        if (_source is not null && target is not null)
        {
            _nodeEditorViewModel.Connect(_source, target);
        }
        
        return Task.CompletedTask;
    }

    private readonly NodeEditorViewModel _nodeEditorViewModel;
    private ConnectorViewModel? _source;
}