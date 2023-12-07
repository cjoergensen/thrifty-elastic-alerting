using Nest;
using Elasticsearch.Net;

namespace Kibana.Alerts.Repositories;
public interface IUserRepository
{
    void UpdateUser();
}
internal class ElasticUserRepository(IConfiguration configuration) : IUserRepository
{
    public void UpdateUser()
    {
        var url = configuration["Elastic:Url"];
        var username = configuration["Elastic:UserName"];
        var password = configuration["Elastic:Password"];
        var client = Extensions.CreateClient(url, username, password);
        client.LowLevel.DoRequest<StringResponse>(
            Elasticsearch.Net.HttpMethod.PUT,
            $"_security/user/{username}",
            PostData.Serializable(new
            {
                // password = password,
                roles = new List<string> { "kibana_admin" }
            }));
    }
}
