using System.ComponentModel;


namespace warkbench.ViewModels;
public interface INameable : INotifyPropertyChanged
{
    string Name { get; set; }
}