﻿using Kibana.Alerts.Model;

namespace Kibana.Alerts.Connectors;
public class WebHookSettings
{
    public string Message { get; set; }
    public string WebHookUrl { get; set; }
}
public sealed class WebHookConnector : IConnector
{
    private readonly IConfiguration configuration;

    public WebHookConnector(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public async Task<bool> TrySend(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default)
    {
        var port = int.Parse(configuration["Smtp:Port"]);
        var host = int.Parse(configuration["Smtp:Host"]);

        var settings = new SmtpSettings();
        configuration.Bind(settings);
        // Call WebHook
        return true;
    }
}