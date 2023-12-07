using Microsoft.Extensions.DependencyInjection;

namespace ThriftyElasticAlerting.Abstractions.Connectors;

public class ConnectorFactory(IServiceProvider services) : IConnectorFactory
{
    public IConnector Create(string connectorKey) => services.GetRequiredKeyedService<IConnector>(connectorKey.ToLowerInvariant());
}
