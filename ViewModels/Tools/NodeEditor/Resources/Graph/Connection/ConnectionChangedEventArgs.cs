namespace warkbench.ViewModels;

public class ConnectionChangedEventArgs(
    ConnectorViewModel sourceConnector,
    ConnectorViewModel targetConnector,
    ConnectionViewModel connection)
{
    public ConnectorViewModel SourceConnector { get; init; } = sourceConnector;
    public ConnectorViewModel TargetConnector { get; init; } = targetConnector;
    public ConnectionViewModel Connection { get; init; } = connection;
}