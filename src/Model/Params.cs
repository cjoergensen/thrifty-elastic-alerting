using System.Text.Json.Serialization;

namespace ThriftyElasticAlerting.Model;

public class Params
{
    [JsonPropertyName("threshold")]
    public decimal? Threshold { get; set; }
    [JsonPropertyName("windowSize")]
    public decimal? WindowSize { get; set; }
    [JsonPropertyName("windowUnit")]
    public string? WindowUnit { get; set; }
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
    [JsonPropertyName("serviceName")]
    public string? ServiceName { get; set; }
}
