using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThriftyElasticAlerting.Abstractions.Connectors;

namespace ThriftyElasticAlerting.Connectors.Smtp;

public static class ServiceCollectionExtensions
{
    public static void AddSmtpConnector(this IServiceCollection services)
    {
        services.AddKeyedTransient<IConnector, Connector>(Connector.Key);
    }
}
