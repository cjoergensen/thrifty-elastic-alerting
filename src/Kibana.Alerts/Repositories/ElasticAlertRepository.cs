using Kibana.Alerts.Model;
using Nest;
using Elasticsearch.Net;
using System.Text.Json;

namespace Kibana.Alerts.Repositories;
public interface IAlertRepository
{
    Task<IEnumerable<Alert>> GetAll();
}

public static class Extensions
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
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .BasicAuthentication(username, password)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        services.AddSingleton(new ElasticClient(settings));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }
}

public class SourceGenSerializer : IElasticsearchSerializer
{
    private readonly JsonSerializerOptions options = new()
    {
            TypeInfoResolver = DocumentContext.Default
    };

    public object Deserialize(Type type, Stream stream) => JsonSerializer.Deserialize(stream, type, options) ?? throw new Exception("Unable to deserialize stream");

    public T Deserialize<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, options) ?? throw new Exception("Unable to deserialize stream");

    public async Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default)
    {
        var result = await JsonSerializer.DeserializeAsync(stream, type, options, cancellationToken);
        return result ?? throw new Exception("Unable to deserialize stream");
    }

    public async Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
    {
        var result = await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
        return result ?? throw new Exception("Unable to deserialize stream");
    }

    public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None) => JsonSerializer.Serialize<T>(stream, data, options);

    public Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None, CancellationToken cancellationToken = default) => JsonSerializer.SerializeAsync<T>(stream, data, options, cancellationToken);
}

internal class ElasticAlertRepository(ElasticClient client, IConfiguration configuration) : IAlertRepository
{
    public async Task<IEnumerable<Alert>> GetAll()
    {
        var kibanaUrl = configuration["Elastic:KibanaUrl"];
        ArgumentException.ThrowIfNullOrWhiteSpace(kibanaUrl, nameof(kibanaUrl));

        if (kibanaUrl.EndsWith('/'))
            kibanaUrl = kibanaUrl[..^1];

        var response = await client.SearchAsync<Document>(s => s
            .Index(Extensions.IndexName)
            .From(0)
            .Size(1000)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        bs => bs.Term(p => p.Type, "alert")
                    )
                )
            ));

        return response.Hits.Select(h =>
        {
            h.Source.Alert.Id = h.Id;

            var url = kibanaUrl + "/app/observability/alerts/rules/" + h.Id.Replace("alert:", "");
            var uri = new Uri(url);
            h.Source.Alert.RuleUrl = uri.ToString();
            return h.Source.Alert;
        });
    }
}