using Kibana.Alerts.Model;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

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
        var settings = new ElasticsearchClientSettings(new Uri(configuration["Elastic:Url"]))
            .DefaultMappingFor<Document>(c => c.IndexName(IndexName))
            .Authentication(new BasicAuthentication(configuration["Elastic:UserName"], configuration["Elastic:Password"]));

        settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
        services.AddSingleton(new ElasticsearchClient(settings));
        services.AddSingleton<IAlertRepository, ElasticAlertRepository>();
    }

}
internal class ElasticAlertRepository(ElasticsearchClient client) : IAlertRepository
{
    public async Task<IEnumerable<Alert>> GetAll()
    {
        var response = await client.SearchAsync<Document>(s => s
            .Index(Extensions.IndexName)
            .From(0)
            .Size(1000)
            .Query(q => q.Term(p => p.Type, "alert")));
        return response.Hits.Select(h => 
        { 
            h.Source.Alert.Id = h.Id; 
            return h.Source.Alert; 
        });
    }

}
