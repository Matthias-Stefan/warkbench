using System.Collections.ObjectModel;


namespace warkbench.ViewModels;

public interface IOutputNodeViewModel
{
    public ObservableCollection<ConnectorViewModel> Outputs { get; }
}