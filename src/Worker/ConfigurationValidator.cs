namespace ThriftyElasticAlerting.Worker;

public static class ConfigurationValidator
{
    public static void ValidateConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ConfigurationException("Configuration is null.");
        }

        EnsureElasticSectionIsValid(configuration);
        EnsureGroupsSectionExists(configuration);
        var groupsSection = configuration.GetSection("Groups");
        foreach (var group in groupsSection.GetChildren())
        {
            var groupName = group.Key; // Get the group name
            var connectorsSection = group.GetSection("Connectors");
            if (!connectorsSection.Exists())
            {
                throw new ConfigurationException($"Group '{groupName}' is missing the 'Connectors' section or it is empty.");
            }

            EnsureAtLeastOneConnectorType(connectorsSection, groupName);
            ValidateSmtpConnector(connectorsSection, configuration, groupName);
            ValidateMsTeamsConnector(connectorsSection, groupName);
        }
    }

    private static void EnsureGroupsSectionExists(IConfiguration configuration)
    {
        var groupsSection = configuration.GetSection("Groups");
        if (!groupsSection.Exists())
        {
            throw new ConfigurationException("At least one groups needs to be present in configuration.");
        }
    }

    private static void EnsureElasticSectionIsValid(IConfiguration configuration)
    {
        var elasticSection = configuration.GetSection("Elastic");
        if (!elasticSection.Exists())
        {
            throw new ConfigurationException($"Required 'Elastic' section is missing or empty. Section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'.");
        }

        var elasticUrl = elasticSection["Url"];
        var elasticUserName = elasticSection["UserName"];
        var elasticPassword = elasticSection["Password"];
        var elasticKibanaUrl = elasticSection["KibanaUrl"];

        if (string.IsNullOrWhiteSpace(elasticUrl) || string.IsNullOrWhiteSpace(elasticUserName) || string.IsNullOrWhiteSpace(elasticPassword) || string.IsNullOrWhiteSpace(elasticKibanaUrl))
        {
            throw new ConfigurationException($"'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'.");
        }
    }

    private static void EnsureAtLeastOneConnectorType(IConfiguration connectorsSection, string groupName)
    {
        if (!connectorsSection.GetSection("Smtp").Exists() && !connectorsSection.GetSection("MsTeams").Exists())
        {
            throw new ConfigurationException($"Group '{groupName}' needs at least one connector of type 'Smtp' or 'MsTeams'.");
        }
    }

    private static void ValidateSmtpConnector(IConfiguration connectorsSection, IConfiguration configuration, string groupName)
    {
        var smtpRecipients = connectorsSection.GetSection("Smtp:Recipients").Get<string[]>();
        if (connectorsSection.GetSection("Smtp").Exists())
        {
            if (smtpRecipients == null || smtpRecipients.Length == 0)
            {
                throw new ConfigurationException($"For Group '{groupName}', the 'Smtp' connector requires a recipients array with one or more items.");
            }

            var smtpServer = configuration.GetSection("SmtpServer");
            if (!smtpServer.Exists())
            {
                throw new ConfigurationException($"Group '{groupName}' is using the 'Smtp' connector, but no SmtpServer settings are present (or empty) in the configuration.");
            }

            var smtpSender = smtpServer["Sender"];
            var smtpHost = smtpServer["Host"];
            var smtpPort = smtpServer["Port"];

            if (string.IsNullOrWhiteSpace(smtpSender) || string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpPort))
            {
                throw new ConfigurationException($"Invalid 'SmtpServer' settings. 'Sender', 'Host', and 'Port' are required.");
            }
        }
    }

    private static void ValidateMsTeamsConnector(IConfiguration connectorsSection, string groupName)
    {
        var teamsWebHookUrl = connectorsSection.GetSection("MsTeams:WebHookUrl").Value;
        if (connectorsSection.GetSection("MsTeams").Exists() && string.IsNullOrWhiteSpace(teamsWebHookUrl))
        {
            throw new ConfigurationException($"For Group '{groupName}', the 'MsTeams' connector requires a WebHookUrl.");
        }
    }
}

[System.Serializable]
public class ConfigurationException : System.Exception
{
    public ConfigurationException() { }
    public ConfigurationException(string message) : base($"Configuration Error: {message}") { }
    public ConfigurationException(string message, System.Exception inner) : base($"Configuration Error: {message}", inner) { }
}