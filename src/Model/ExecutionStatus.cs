using System.Text.Json.Serialization;

namespace ThriftyElasticAlerting.Model;

public class ExecutionStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "";
    [JsonPropertyName("lastExecutionDate")]
    public DateTimeOffset LastExecutionDate { get; set; }
    [JsonPropertyName("lastDuration")]
    public long LastDurationMs { get; set; }
    //[JsonPropertyName("error")]
    //public string? Error { get; set; }
    [JsonPropertyName("warning")]
    public string? Warning { get; set; }
}
