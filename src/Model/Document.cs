using System.Text.Json.Serialization;

namespace ThriftyElasticAlerting.Model;
public class Document
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("alert")]
    public Alert Alert { get; set; } = new();
}
