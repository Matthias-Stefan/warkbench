using System.Collections.ObjectModel;


namespace warkbench.ViewModels;
public interface IGraphContainer
{
    ObservableCollection<NodeViewModel> Nodes { get; }
    ObservableCollection<ConnectionViewModel> Connections { get; }
}