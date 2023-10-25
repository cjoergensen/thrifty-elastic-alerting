using System.Text.Json.Serialization;

namespace Elastic.Alerts.Model;

public class ExecutionStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; }
    [JsonPropertyName("lastExecutionDate")]
    public DateTimeOffset LastExecutionDate { get; set; }
    [JsonPropertyName("lastDuration")]
    public long LastDurationMs { get; set; }
    [JsonPropertyName("error")]
    public string? Error { get; set; }
    [JsonPropertyName("warning")]
    public string? Warning { get; set; }
}

public class Alert
{
    public string Id { get; set; }
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }
    [JsonPropertyName("running")]
    public bool Running { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }
    [JsonPropertyName("executionStatus")]
    public ExecutionStatus ExecutionStatus { get; set; }
    [JsonPropertyName("nextRun")]
    public DateTimeOffset NextRun { get; set; }
}
public class Document
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("alert")]
    public Alert Alert { get; set; }
}