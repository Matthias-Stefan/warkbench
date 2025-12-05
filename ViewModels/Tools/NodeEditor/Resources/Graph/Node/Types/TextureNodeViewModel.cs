using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using warkbench.Brushes;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class TextureNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public TextureNodeViewModel(TextureNodeModel model)
        : base(model)
    {
        BorderColor = NodeBrushes.Texture;
        SelectedColor = NodeBrushes.Texture;

        Outputs.CollectionChanged += OnOutputsChanged;
        if (model.Outputs.Count == 0)
        {
            Outputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid  = System.Guid.NewGuid() }, this));    
        }
        else
        {
            foreach (var connector in model.Outputs)
            {
                Outputs.Add(new ConnectorViewModel(connector, this));
            }
        }
    }
    
    public override void HandleConnected(object? sender, ConnectionChangedEventArgs? args)
    {
    }

    public override void HandleDisconnected(object? sender, ConnectionChangedEventArgs? args)
    {
    }

    public string Value
    {
        get => TextureModel.Value;
        set
        {
            if (TextureModel.Value == value)
            {
                return;
            }

            TextureModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                TextureModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                TextureModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }

    public TextureNodeModel TextureModel => (Model as TextureNodeModel)!;

    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}