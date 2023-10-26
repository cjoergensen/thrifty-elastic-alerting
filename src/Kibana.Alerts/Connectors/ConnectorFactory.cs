using Kibana.Alerts.Model;

namespace Kibana.Alerts.Connectors;

public interface IConnector
{
    Task<bool> TrySend(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default);
}
public class ConnectorFactory(IServiceProvider services)
{
    public IConnector Create(string connectorType) => services.GetRequiredKeyedService<IConnector>(connectorType.ToLower());
}
