using Nest;
using ThriftyElasticAlerting.Model;
using Microsoft.Extensions.Configuration;
using System;

namespace ThriftyElasticAlerting.Repositories;

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

