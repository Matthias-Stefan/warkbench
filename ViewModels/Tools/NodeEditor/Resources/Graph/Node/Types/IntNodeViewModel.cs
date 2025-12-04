using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using warkbench.Models;


namespace warkbench.ViewModels;
public partial class IntNodeViewModel : NodeViewModel, IOutputNodeViewModel
{
    public IntNodeViewModel(IntNodeModel model)
        : base(model)
    {
        BorderColor = new SolidColorBrush(Colors.IndianRed);
        SelectedColor = new SolidColorBrush(Colors.IndianRed);
        
        Outputs.CollectionChanged += OnOutputsChanged;
        if (model.Outputs.Count == 0)
        {
            Outputs.Add(new ConnectorViewModel(new ConnectorModel{ Guid = System.Guid.NewGuid() }, this));    
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
    
    public int Value
    {
        get => IntModel.Value;
        set
        {
            if (IntModel.Value == value)
            {
                return;
            }

            IntModel.Value = value;
            OnPropertyChanged();
        }
    }
    
    private void OnOutputsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.OldItems)
            {
                IntModel.Outputs.Remove(connectorViewModel.Model);    
            }
        }

        if (args.NewItems is not null)
        {
            foreach (ConnectorViewModel connectorViewModel in args.NewItems)
            {
                IntModel.Outputs.Add(connectorViewModel.Model);    
            }
        }
    }
    
    public IntNodeModel IntModel => (Model as IntNodeModel)!;
    public ObservableCollection<ConnectorViewModel> Outputs { get; } = [];
}