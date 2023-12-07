namespace ThriftyElasticAlerting.Connectors.Smtp;

public class Settings
{
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public List<string> Recipients { get; set; } = [];
}
