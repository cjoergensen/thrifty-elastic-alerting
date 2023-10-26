using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kibana.Alerts.Connectors;
public class SmtpSettings
{
    public string Subject { get; set; }
    public string Body { get; set; }
    public List<string> Audience {  get; set; }
}
public sealed class SmtpConnector : IConnector
{
    private readonly IConfiguration configuration;

    public SmtpConnector(IConfiguration configuration)
    {
        this.configuration = configuration;
    }
    public async Task<bool> TrySend(IConfigurationSection configurationSection)
    {
        var port = int.Parse(configuration["Smtp:Port"]);
        var host = int.Parse(configuration["Smtp:Host"]);
       
        var settings = new SmtpSettings();
        configuration.Bind(settings);
        // Call SMTP
        return true;
    }
}
