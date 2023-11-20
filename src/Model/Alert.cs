using System.Text.Json.Serialization;

namespace ThriftyElasticAlerting.Model;

public class Alert
{
    public string Id { get; set; } = "";

    [JsonIgnore]
    public string RuleUrl { get; set; } = "";
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
    [JsonPropertyName("running")]
    public bool Running { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
    [JsonPropertyName("alertTypeId")]
    public string AlertType { get; set; } = "";

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
    [JsonPropertyName("executionStatus")]
    public ExecutionStatus ExecutionStatus { get; set; } = new();
    [JsonPropertyName("params")]
    public Params? Params { get; set; }
    [JsonPropertyName("nextRun")]
    public DateTimeOffset NextRun { get; set; }
}
