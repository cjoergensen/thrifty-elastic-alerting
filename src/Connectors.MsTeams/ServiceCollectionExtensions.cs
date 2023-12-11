using Microsoft.Extensions.DependencyInjection;
using ThriftyElasticAlerting.Abstractions.Connectors;

namespace ThriftyElasticAlerting.Connectors.MsTeams;

public static class ServiceCollectionExtensions
{
    public static void AddMsTeamsConnector(this IServiceCollection services)
    {
        services.AddHttpClient<Connector>();
        services.AddKeyedTransient<IConnector, Connector>(Connector.Key.ToLowerInvariant());
    }

}
