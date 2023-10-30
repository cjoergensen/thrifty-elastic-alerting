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

        var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        var settings = new ConnectionSettings(pool, sourceSerializer: (_, _) => new SourceGenSerializer())
            .DefaultIndex(IndexName)
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
            .BasicAuthentication(configuration["Elastic:UserName"], configuration["Elastic:Password"])
            .ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        services.AddSingleton(new ElasticClient(settings));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }

}
internal class ElasticAlertRepository(ElasticClient client, IConfiguration configuration) : IAlertRepository
{
    public async Task<IEnumerable<Alert>> GetAll()
    {
        var publicUrl = configuration["Elastic:PublicUrl"];
        if (publicUrl.EndsWith('/'))
            publicUrl = publicUrl[..^1];

        var response = await client.SearchAsync<Document>(s => s
            .Index(Extensions.IndexName)
            .From(0)
            .Size(1000)
            .Query(q => q.Term(p => p.Type, "alert")));
        return response.Hits.Select(h => 
        { 
            h.Source.Alert.Id = h.Id;

            var url = publicUrl + "/app/observability/alerts/rules/" + h.Id.Replace("alert:", "");
            var uri = new Uri(url);
            h.Source.Alert.RuleUrl = uri.ToString();
            return h.Source.Alert; 
        });
    }
}


public class SourceGenSerializer : IElasticsearchSerializer
{

    private readonly JsonSerializerOptions options = new()
    {
            TypeInfoResolver = DocumentContext.Default
    };

    public object Deserialize(Type type, Stream stream) => JsonSerializer.Deserialize(stream, type, options);

    public T Deserialize<T>(Stream stream) => JsonSerializer.Deserialize<T>(stream, options);

    public Task<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default) => JsonSerializer.DeserializeAsync(stream, type, options, cancellationToken).AsTask();

    public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) => JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken).AsTask();

    public void Serialize<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None)
    {
        throw new NotImplementedException();
    }

    public Task SerializeAsync<T>(T data, Stream stream, SerializationFormatting formatting = SerializationFormatting.None, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
