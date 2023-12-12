using HandlebarsDotNet;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using ThriftyElasticAlerting.Abstractions.Connectors;
using ThriftyElasticAlerting.Model;

namespace ThriftyElasticAlerting.Connectors.MsTeams;
public sealed class Connector(HttpClient httpClient, IHandlebars handlebars) : IConnector
{
    public const string Key = "MsTeams";

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
        var settings = new Settings();
        configurationSection.Bind(settings);

        ArgumentException.ThrowIfNullOrWhiteSpace(settings.WebHookUrl, nameof(settings.WebHookUrl));

        var cardJson = string.IsNullOrWhiteSpace(settings.MessageCardJson) ? DefaultMessageCardJson : settings.MessageCardJson;
        var cardTemplate = handlebars.Compile(cardJson);
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