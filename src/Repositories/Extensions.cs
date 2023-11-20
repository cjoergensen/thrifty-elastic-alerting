using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ThriftyElasticAlerting.Repositories;

public static class ServiceCollectionExtensions
{
    public const string IndexName = ".kibana_alerting_cases";
    public static void AddElasticClient(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["Elastic:Url"];
        var username = configuration["Elastic:UserName"];
        var password = configuration["Elastic:Password"];

        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var pool = new SingleNodeConnectionPool(new Uri(url));
        var settings = new ConnectionSettings(pool, sourceSerializer: (_, _) => new SourceGenSerializer())
            .DefaultIndex(IndexName)
            .BasicAuthentication(username, password);

        services.AddSingleton(new ElasticClient(settings));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }
}
