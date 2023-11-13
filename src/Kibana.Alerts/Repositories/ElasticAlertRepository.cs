using Kibana.Alerts.Model;
using Nest;

namespace Kibana.Alerts.Repositories;
public interface IAlertRepository
{
    Task<IEnumerable<Alert>> GetAll();
}

internal class ElasticAlertRepository: IAlertRepository
{
    private readonly IConfiguration configuration;
    ElasticClient client;
    public ElasticAlertRepository(IConfiguration configuration)
    {
        var url = configuration["Elastic:Url"];
        var username = "alerting";
        var password = "a13rtin9";
        client = Extensions.CreateClient(url, username, password);
        this.configuration = configuration;
    }
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