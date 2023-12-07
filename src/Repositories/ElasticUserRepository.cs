using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;

namespace ThriftyElasticAlerting.Repositories;
public interface IUserRepository
{
    void UpdateUser();
}
internal class ElasticUserRepository(ElasticClient client, IConfiguration configuration) : IUserRepository
{
    public void UpdateUser()
    {
        var username = configuration["Elastic:UserName"];
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

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