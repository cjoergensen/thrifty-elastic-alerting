using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;

namespace ThriftyElasticAlerting.Repositories;

public static class Extensions
{
    public const string IndexName = ".kibana_alerting_cases";
    public static void AddElasticClient(this IServiceCollection services, IConfiguration configuration)
    {
        var url = configuration["Elastic:Url"];
        var username = configuration["Elastic:UserName"];
        var password = configuration["Elastic:Password"];
        services.AddSingleton(CreateClient(url, username, password));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }

    public static ElasticClient CreateClient(string? url, string? username, string? password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var pool = new SingleNodeConnectionPool(new Uri(url));
        var settings = new ConnectionSettings(pool, sourceSerializer: (_, _) => new SourceGenSerializer())
            .DefaultIndex(IndexName)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .BasicAuthentication(username, password)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll);
        return new ElasticClient(settings);
    }
}
