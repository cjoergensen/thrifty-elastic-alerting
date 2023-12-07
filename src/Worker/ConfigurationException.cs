namespace ThriftyElasticAlerting.Worker;

[System.Serializable]
public class ConfigurationException : Exception
{
    public ConfigurationException() { }
    public ConfigurationException(string message) : base($"Configuration Error: {message}") { }
    public ConfigurationException(string message, System.Exception inner) : base($"Configuration Error: {message}", inner) { }
}