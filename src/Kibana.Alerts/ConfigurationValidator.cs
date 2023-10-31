namespace Kibana.Alerts;

public static class ConfigurationValidator
{
    public static void ValidateConfiguration(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new Exception("Configuration is null.");
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
                throw new Exception($"Group '{groupName}' is missing the 'Connectors' section or it is empty.");
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
            throw new Exception("At least one groups needs to be present in configuration.");
        }
    }

    private static void EnsureElasticSectionIsValid(IConfiguration configuration)
    {
        var elasticSection = configuration.GetSection("Elastic");
        if (!elasticSection.Exists())
        {
            throw new Exception($"Required 'Elastic' section is missing or empty. Section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'.");
        }

        var elasticUrl = elasticSection["Url"];
        var elasticUserName = elasticSection["UserName"];
        var elasticPassword = elasticSection["Password"];
        var elasticKibanaUrl = elasticSection["KibanaUrl"];

        if (string.IsNullOrWhiteSpace(elasticUrl) || string.IsNullOrWhiteSpace(elasticUserName) || string.IsNullOrWhiteSpace(elasticPassword) || string.IsNullOrWhiteSpace(elasticKibanaUrl))
        {
            throw new Exception($"'Elastic' section must contain 'Url', 'UserName', 'Password', and 'KibanaUrl'.");
        }
    }

    private static void EnsureAtLeastOneConnectorType(IConfiguration connectorsSection, string groupName)
    {
        if (!connectorsSection.GetSection("Smtp").Exists() && !connectorsSection.GetSection("MsTeams").Exists())
        {
            throw new Exception($"Group '{groupName}' needs at least one connector of type 'Smtp' or 'MsTeams'.");
        }
    }

    private static void ValidateSmtpConnector(IConfiguration connectorsSection, IConfiguration configuration, string groupName)
    {
        var smtpRecipients = connectorsSection.GetSection("Smtp:Recipients").Get<string[]>();
        if (connectorsSection.GetSection("Smtp").Exists())
        {
            if (smtpRecipients == null || smtpRecipients.Length == 0)
            {
                throw new Exception($"For Group '{groupName}', the 'Smtp' connector requires a recipients array with one or more items.");
            }

            var smtpServer = configuration.GetSection("SmtpServer");
            if (!smtpServer.Exists())
            {
                throw new Exception($"Group '{groupName}' is using the 'Smtp' connector, but no SmtpServer settings are present (or empty) in the configuration.");
            }

            var smtpSender = smtpServer["Sender"];
            var smtpHost = smtpServer["Host"];
            var smtpPort = smtpServer["Port"];

            if (string.IsNullOrWhiteSpace(smtpSender) || string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpPort))
            {
                throw new Exception($"Invalid 'SmtpServer' settings. 'Sender', 'Host', and 'Port' are required.");
            }
        }
    }

    private static void ValidateMsTeamsConnector(IConfiguration connectorsSection, string groupName)
    {
        var teamsWebHookUrl = connectorsSection.GetSection("MsTeams:WebHookUrl").Value;
        if (connectorsSection.GetSection("MsTeams").Exists() && string.IsNullOrWhiteSpace(teamsWebHookUrl))
        {
            throw new Exception($"For Group '{groupName}', the 'MsTeams' connector requires a WebHookUrl.");
        }
    }
}