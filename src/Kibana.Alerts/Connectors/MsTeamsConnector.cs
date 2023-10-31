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
      "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
      "type": "AdaptiveCard",
      "version": "1.6",
      "body": [
        {
          "type": "TextBlock",
          "weight": "Bolder",
          "text": "Important Update"
        },
        {
          "type": "TextBlock",
          "text": "The alert '{{Name}}' has changed status to '{{ExecutionStatus.Status}}'.",
          "wrap": true
        }
      ],
      "actions": [
        {
          "type": "Action.OpenUrl",
          "title": "View Details",
          "url": "{{RuleUrl}}"
        }
      ]
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