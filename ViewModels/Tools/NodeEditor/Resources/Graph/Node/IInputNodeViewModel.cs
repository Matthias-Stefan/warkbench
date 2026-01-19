using System.Collections.ObjectModel;


namespace warkbench.ViewModels;

public interface IInputNodeViewModel
{
    public ObservableCollection<ConnectorViewModel> Inputs { get; }
}