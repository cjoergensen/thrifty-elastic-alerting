using HandlebarsDotNet;
using Kibana.Alerts.Model;
using System.Net.Http.Headers;

namespace Kibana.Alerts.Connectors;
public class MsTeamsSettings
{
    public string MessageCardJson { get; set; }
    public string WebHookUrl { get; set; }
}

public sealed class MsTeamsConnector(IConfiguration configuration, HttpClient httpClient, IHandlebars handlebars) : IConnector
{
    private readonly IConfiguration configuration = configuration;
    private readonly HttpClient httpClient = httpClient;
    private const string MediaType = "application/json";
    private const string DefaultMessageCardJson = """
    {
      "type": "AdaptiveCard",
      "body": [
        {
          "type": "TextBlock",
          "size": "Medium",
          "weight": "Bolder",
          "text": "⚠ Alert Status Update - {{Name}} has changed state to: {{ExecutionStatus.Status}} ⚠"
        },
        {
          "type": "TextBlock",
          "text": "There has been a change in the status of alert "<strong>{{Name}}</strong>." It is now <strong>{{ExecutionStatus.Status}}</strong>.</p><p>For detailed information regarding this alert's status change, please click the view details button.",
          "wrap": true
        }
      ],
      "actions": [
        {
          "type": "Action.OpenUrl",
          "title": "View details",
          "url": "{{RuleUrl}}"
        }
      ],
      "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
      "version": "1.6"
    }
    """;

    public async Task Send(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default)
    {
        var settings = new MsTeamsSettings();
        configurationSection.Bind(settings);

        var cardTemplate = handlebars.Compile(settings.MessageCardJson ?? DefaultMessageCardJson);
        var card = cardTemplate(alert);

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));

        string message = $$"""
        {
            "type": "message",
            "attachments": [
                {
                    "contentType": "application/vnd.microsoft.card.adaptive",
                    "contentUrl": null,
                    "content": {{card}}
                }
            ]
        }
        """;


        var content = new StringContent(message, System.Text.Encoding.UTF8, MediaType);
        var response = await httpClient.PostAsync(settings.WebHookUrl, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}