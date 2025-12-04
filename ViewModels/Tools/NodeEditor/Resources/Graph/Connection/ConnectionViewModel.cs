using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class ConnectionViewModel : ObservableObject
{
    // TODO:
    public ConnectionViewModel(ConnectionModel model, ConnectorViewModel source, ConnectorViewModel target)
    {
        Model = model;
        Source = source;
        Target = target;
    }

    public ConnectorViewModel Source
    {
        get => _source;
        set
        {
            SetProperty(ref _source, value);
            Model.Source = _source.Model;
        }
    }
    
    public ConnectorViewModel Target
    {
        get => _target;
        set
        {
            SetProperty(ref _target, value);
            Model.Target = _target.Model;
        }
    }
    
    public ConnectionModel Model { get; }
    
    private ConnectorViewModel _source;
    private ConnectorViewModel _target;
}