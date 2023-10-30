using Kibana.Alerts.Model;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
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
        if (string.IsNullOrWhiteSpace(configuration["Elastic:PublicUrl"]))
            throw new Exception("Environment variable Elastic__PublicUrl was not specified");
        if (string.IsNullOrWhiteSpace(configuration["Elastic:UserName"]))
            throw new Exception("Environment variable Elastic__UserName was not specified");
        if (string.IsNullOrWhiteSpace(configuration["Elastic:Password"]))
            throw new Exception("Environment variable Elastic__Password was not specified");

        var pool = new StaticNodePool(new Uri[] { new Uri(configuration["Elastic:PublicUrl"]) });
        var settings = new ElasticsearchClientSettings(nodePool: pool, sourceSerializer: (x,y) => new SourceContextSerializer(y))
            .DefaultMappingFor<Document>(c => c.IndexName(IndexName))
            .Authentication(new BasicAuthentication(configuration["Elastic:UserName"], configuration["Elastic:Password"]));
        
        settings.ServerCertificateValidationCallback(Elasticsearch.Net.CertificateValidations.AllowAll);
        services.AddSingleton(new ElasticsearchClient(settings));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }
}
public class SourceContextSerializer : Serializer
{
    private JsonSerializerOptions options;
    private JsonSerializerOptions sourceGenOptions;

    public SourceContextSerializer(IElasticsearchClientSettings settings) : base()
    {
        sourceGenOptions = new JsonSerializerOptions
        {
            TypeInfoResolver = DocumentContext.Default
        };
    }

    public override object Deserialize(Type type, Stream stream) =>
        JsonSerializer.Deserialize(stream, type, sourceGenOptions);

    public override T Deserialize<T>(Stream stream) =>
        JsonSerializer.Deserialize<T>(stream, sourceGenOptions);

    public override async ValueTask<object> DeserializeAsync(Type type, Stream stream, CancellationToken cancellationToken = default) =>
        await JsonSerializer.DeserializeAsync(stream, type, sourceGenOptions, cancellationToken);

    public override async ValueTask<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) =>
        await JsonSerializer.DeserializeAsync<T>(stream, sourceGenOptions, cancellationToken);

    public override void Serialize<T>(T data, Stream stream, Elastic.Transport.SerializationFormatting formatting = Elastic.Transport.SerializationFormatting.None)
    {
        throw new NotImplementedException();
    }

    public override Task SerializeAsync<T>(T data, Stream stream, Elastic.Transport.SerializationFormatting formatting = Elastic.Transport.SerializationFormatting.None, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
internal class ElasticAlertRepository(ElasticsearchClient client, IConfiguration configuration) : IAlertRepository
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
