using Nest;
using Elasticsearch.Net;

namespace Kibana.Alerts.Repositories;
public interface IUserRepository
{
    void UpdateUser();
}
internal class ElasticUserRepository : IUserRepository
{
    ElasticClient client;
    public ElasticUserRepository(IConfiguration configuration)
    {
        var url = configuration["Elastic:Url"];
        var username = configuration["Elastic:UserName"];
        var password = configuration["Elastic:Password"];
        client = Extensions.CreateClient(url, username, password);
    }
    public void UpdateUser()
    {
        client.LowLevel.DoRequest<StringResponse>(
            Elasticsearch.Net.HttpMethod.PUT,
            "_security/user/alerting",
            PostData.Serializable(new
            {
                password = "a13rtin9",
                roles = new List<string> { "kibana_admin" }
            }));
    }
}
