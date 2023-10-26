using Kibana.Alerts.Model;
using System.Net.Http.Headers;

namespace Kibana.Alerts.Connectors;
public class MsTeamsSettings
{
    public string MessageCardJson { get; set; }
    public string WebHookUrl { get; set; }
}

public sealed class MsTeamsConnector(IConfiguration configuration, HttpClient httpClient) : IConnector
{
    private readonly IConfiguration configuration = configuration;
    private readonly HttpClient httpClient = httpClient;
    private const string MediaType = "application/json";

    public async Task<bool> TrySend(Alert alert, IConfigurationSection configurationSection, CancellationToken cancellationToken = default)
    {        
        var settings = new MsTeamsSettings();
        configurationSection.Bind(settings);

        var template = new AdaptiveCards.Templating.AdaptiveCardTemplate(settings.MessageCardJson);
        var card = template.Expand(alert);

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
        var content = new StringContent(card, System.Text.Encoding.UTF8, MediaType);
        var response = await httpClient.PostAsync(settings.WebHookUrl, content, cancellationToken);
        return response.IsSuccessStatusCode;


        // working request:
        // {
        //    "type": "message",
        //    "attachments": [
        //      {
        //        "contentType": "application/vnd.microsoft.card.adaptive",
        //        "contentUrl": null,
        //        "content": {
        //            "type": "AdaptiveCard",
        //            "body": [
        //                {
        //                "type": "TextBlock",
        //                    "size": "Medium",
        //                    "weight": "Bolder",
        //                    "text": "Alarm1 is now resolved"
        //                }
        //            ],
        //            "actions": [
        //                {
        //                "type": "Action.OpenUrl",
        //                    "title": "View details",
        //                    "url": "https://mdm-dev.seas.local:30443/kibana/app/observability/alerts/rules/c20e6440-7313-11ee-a35d-8db117b46165"
        //                }
        //            ],
        //            "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
        //            "version": "1.6"
        //        }
        //      }
        //    ]
        // }
    }
}