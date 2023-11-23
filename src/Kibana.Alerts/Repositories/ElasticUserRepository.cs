using Nest;
using Elasticsearch.Net;

namespace Kibana.Alerts.Repositories;
public interface IUserRepository
{
    void UpdateUser();
    string GetPassword();
}
internal class ElasticUserRepository : IUserRepository
{
    ElasticClient client;
    string alertingPassword;
    public ElasticUserRepository(IConfiguration configuration)
    {
        var url = configuration["Elastic:Url"];
        var username = configuration["Elastic:UserName"];
        var password = configuration["Elastic:Password"];
        client = Extensions.CreateClient(url, username, password);
        alertingPassword = Guid.NewGuid().ToString().Replace("-", "");
    }
    public string GetPassword() => alertingPassword;

    public void UpdateUser()
    {
        client.LowLevel.DoRequest<StringResponse>(
            Elasticsearch.Net.HttpMethod.PUT,
            "_security/user/alerting",
            PostData.Serializable(new
            {
                password = alertingPassword,
                roles = new List<string> { "kibana_admin" }
            }));
    }
}
